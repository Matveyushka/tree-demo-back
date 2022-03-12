using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Structuralist.M1;

public class Builder
{
    private TreeNode[] ConvertBuilderTree(BuilderTreeNode sourceTree)
    {
        int nextNodeIndex = 1;
        var treeList = new List<TreeNode>();

        var queue = new Queue<BuilderTreeNode>();

        queue.Enqueue(sourceTree);

        while (queue.Count != 0)
        {
            var currentNode = queue.Dequeue();
            treeList.Add(new TreeNode()
            {
                type = currentNode.Type,
                children = new List<int>()
            });
            if (currentNode.Content != null)
            {
                var content = new StringBuilder();
                foreach (var current in currentNode.Content)
                {
                    foreach (var module in currentNode.ModuleList)
                    {
                        content.Append(module.name + "[" + module.index + "]" + ".");
                    }
                    content.Append(current.Name + ": ");
                    foreach (var item in current.Items)
                    {
                        content.Append(item + " ");
                    }
                    content.Append("; ");
                }
                treeList.Last().content = content.ToString();
            }

            foreach (var child in currentNode.Children)
            {
                queue.Enqueue(child);
                treeList.Last().children.Add(nextNodeIndex++);
            }
        }

        return treeList.ToArray();
    }

    /*private bool IsCurrentModule(BuilderTreeNode node, List<string> moduleList)
    {
        bool isCurrentModule = true;
        if (node.ModuleList.Count == moduleList.Count)
        {
            for (int moduleNameIndex = 0; moduleNameIndex != moduleList.Count; moduleNameIndex++)
            {
                if (node.ModuleList[moduleNameIndex] != moduleList[moduleNameIndex])
                {
                    isCurrentModule = false;
                    break;
                }
            }
        }
        else
        {
            isCurrentModule = false;
        }
        return isCurrentModule;
    }*/
    private BuilderTreeNode GenerateBasicTree(ModuleM1 module, List<ModuleInstanceName>? moduleList = null, int moduleNumber = 0)
    {
        var tree = new BuilderTreeNode()
        {
            Type = TreeNodeType.AND,
            Children = new List<BuilderTreeNode>(),
            Content = module.ClassificationFeatures,
            ModuleList = (moduleList ?? new List<ModuleInstanceName>())
                .Concat(new List<ModuleInstanceName>() { new ModuleInstanceName {
                        name = module.Name,
                        index = moduleNumber
                    } }).ToList()
        };

        var pointer = tree;

        for (int featureIndex = 0; featureIndex != module.ClassificationFeatures.Count; featureIndex++)
        {
            var choiceNode = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Children = module.ClassificationFeatures[featureIndex].Items.Select(item => new BuilderTreeNode()
                {
                    Type = TreeNodeType.OR,
                    Children = new List<BuilderTreeNode>(),
                    Content = new List<ItemsGroup>() {
                            new ItemsGroup() {
                                Name = module.ClassificationFeatures[featureIndex].Name,
                                Items = new List<string>() { item }
                            }
                        },
                    ModuleList = tree.ModuleList
                }).ToList(),
                ModuleList = tree.ModuleList
            };
            pointer.Children.Add(choiceNode);
            if (featureIndex != module.ClassificationFeatures.Count - 1)
            {
                var nextAndNode = new BuilderTreeNode()
                {
                    Type = TreeNodeType.AND,
                    Children = new List<BuilderTreeNode>(),
                    Content = module.ClassificationFeatures.Where((group, index) => index > featureIndex).ToList(),
                    ModuleList = tree.ModuleList
                };
                pointer.Children.Add(nextAndNode);
                pointer = nextAndNode;
            }
        }

        return tree;
    }

    private void DivideTreeByConstraintConditions(BuilderTreeNode tree, Constraint constraint)
    {
        var constraintOptions = constraint
            .Conditions
            .Find(condition => condition.ClassificationFeatureName == tree.Content[0].Name)
            .ConditionOptions
            .ToList();

        var left = new BuilderTreeNode(tree);

        left.Content[0].Items = left.Content[0]
            .Items
            .Where(item => constraintOptions.FindIndex(option => option == item) != -1)
            .ToList();

        left.Children[0].Children = left.Children[0]
            .Children
            .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Items[0]) != -1)
            .ToList();

        if (left.Children[0].Children.Count > 0)
        {
            var right = new BuilderTreeNode(tree);

            right.Children[0].Children = right.Children[0]
                .Children
                .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Items[0]) == -1)
                .ToList();

            right.Content[0].Items = right.Content[0]
                .Items
                .Where(item => constraintOptions.FindIndex(option => option == item) == -1)
                .ToList();

            tree.Content = null;
            tree.Type = TreeNodeType.OR;
            tree.Children = new List<BuilderTreeNode>()
                                {
                                    left
                                };

            if (right.Children[0].Children.Count > 0)
            {
                tree.Children.Add(right);
            }
        }
    }

    private void ApplyFeatureConstraint(BuilderTreeNode tree, Constraint constraint)
    {
        if (constraint is not null && constraint.Consequence is ConstraintFeatureConsequence consequence)
        {
            if (tree.Type == TreeNodeType.AND)
            {
                var topNodeFeaturePresentsInCondition = constraint
                    .Conditions
                    .FindIndex(condition => condition.ClassificationFeatureName == tree.Content[0].Name) != -1;

                var topNodeFeaturePresentsInConqecuence = consequence
                   .FeatureName == tree.Content[0].Name;

                if (topNodeFeaturePresentsInCondition)
                {
                    DivideTreeByConstraintConditions(tree, constraint);
                    if (tree.Children[0].Children.Count > 1)
                    {
                        ApplyFeatureConstraint(tree.Children[0].Children[1], constraint);
                    }
                }
                else if (tree.Children.Count > 1)
                {
                    ApplyFeatureConstraint(tree.Children[1], constraint);
                }
                if (topNodeFeaturePresentsInConqecuence)
                {
                    tree.Content[0].Items = tree.Content[0].Items.Where(item =>
                        consequence
                        .ValidOptions.FindIndex(option => option == item) != -1).ToList();

                    tree.Children[0].Children = tree.Children[0].Children.Where((child) =>
                        consequence
                        .ValidOptions.FindIndex(option => child.Content[0].Items[0] == option) != -1
                    ).ToList();
                }
            }
            else
            {
                for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                {
                    ApplyFeatureConstraint(tree.Children[childIndex], constraint);
                }
            }
        }
    }

    private bool IsConstraintFullyApplied(
        string currentItemsGroupName,
        Constraint constraint,
        BuilderTreeNode tree,
        List<ModuleM1> modules)
    {
        var currentModule = modules.Find(module => module.Name == tree.ModuleList.Last().name);
        var consequence = constraint.Consequence as ConstraintModuleConsequence;

        var lastConstraintItemsGroupIndex = 0;
        for (var i = 0; i != constraint.Conditions.Count; i++)
        {
            var optionIndex = currentModule
                .ClassificationFeatures
                .FindIndex(x => x.Name == constraint.Conditions[i].ClassificationFeatureName);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex;
            }
        }

        var expressionVariables = consequence.Expression.GetVariables();
        for (var i = 0; i != expressionVariables.Count; i++)
        {
            var optionIndex = currentModule
                .ClassificationFeatures
                .FindIndex(x => x.Name == expressionVariables[i]);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex;
            }
        }

        var currentItemsGroupIndex = currentModule.ClassificationFeatures.FindIndex(itemsGroup => itemsGroup.Name == currentItemsGroupName);

        return currentItemsGroupIndex >= lastConstraintItemsGroupIndex;
    }

    private void ApplyModuleConstraint(BuilderTreeNode tree, Constraint constraint, List<ModuleM1> modules)
    {
        if (constraint is not null && constraint.Consequence is ConstraintModuleConsequence consequence)
        {
            if (tree.Type == TreeNodeType.AND)
            {
                var topNodeFeaturePresentsInCondition = constraint
                    .Conditions
                    .FindIndex(condition => condition.ClassificationFeatureName == tree.Content[0].Name) != -1;

                var topNodeFeaturePresentsInConsequence = consequence
                    .Expression
                    .IsVariableUsed(tree.Content[0].Name);

                if (topNodeFeaturePresentsInCondition)
                {
                    var condition = constraint
                        .Conditions
                        .First(condition => condition.ClassificationFeatureName == tree.Content[0].Name);

                    var conditionIsFullfilled = (tree.Content[0].Items.All(conditionOption => condition.ConditionOptions.Contains(conditionOption)));

                    if (conditionIsFullfilled && topNodeFeaturePresentsInConsequence)
                    {
                        DivideTreeByModuleConstraint(tree);
                        for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                        {
                            if (tree.Children[childIndex].SavedValues.Count >= consequence.Expression.GetVariableAmount() &&
                                IsConstraintFullyApplied(tree.Content[0].Name, constraint, tree, modules))
                            {
                                MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, consequence);
                            }
                            else if (tree.Children[childIndex].Children.Count > 1)
                            {
                                ApplyModuleConstraint(tree.Children[childIndex].Children[1], constraint, modules);
                            }
                        }
                    }
                    else if (conditionIsFullfilled)
                    {
                        tree.Children[0].SavedValues = tree.SavedValues;
                        if (tree.SavedValues.Count == consequence.Expression.GetVariableAmount() &&
                            IsConstraintFullyApplied(tree.Content[0].Name, constraint, tree, modules))
                        {
                            MarkToGenerateSubModules(tree.Children[0], modules, consequence);
                        }
                        else
                        {
                            for (var i = 1; i < tree.Children.Count; i++)
                            {
                                ApplyModuleConstraint(tree.Children[i], constraint, modules);
                            }
                        }
                    }
                    else if (topNodeFeaturePresentsInConsequence)
                    {
                        DivideTreeByConstraintConditions(tree, constraint);
                        ApplyModuleConstraint(tree.Children[0], constraint, modules);
                    }
                    else
                    {
                        DivideTreeByConstraintConditions(tree, constraint);
                        if (tree.Children[0].Children.Count > 1 && tree.Type == TreeNodeType.AND)
                        {
                            ApplyModuleConstraint(tree.Children[0].Children[1], constraint, modules);
                        }
                        else if (tree.Type == TreeNodeType.OR)
                        {
                            ApplyModuleConstraint(tree.Children[0], constraint, modules);
                        }
                    }
                }
                else if (topNodeFeaturePresentsInConsequence)
                {
                    DivideTreeByModuleConstraint(tree);
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        if (tree.Children[childIndex].SavedValues.Count == consequence.Expression.GetVariableAmount() &&
                                IsConstraintFullyApplied(tree.Content[0].Name, constraint, tree, modules))
                        {
                            MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, consequence);
                        }
                        else
                        {
                            for (var i = 1; i < tree.Children.Count; i++)
                            {
                                ApplyModuleConstraint(tree.Children[i], constraint, modules);
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 1; i < tree.Children.Count; i++)
                    {
                        ApplyModuleConstraint(tree.Children[i], constraint, modules);
                    }
                }
            }
            else
            {
                for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                {
                    ApplyModuleConstraint(tree.Children[childIndex], constraint, modules);
                }
            }
        }
    }

    void MarkToGenerateSubModules(BuilderTreeNode node, List<ModuleM1> modules, ConstraintModuleConsequence consequence)
    {
        var aaa = new Dictionary<string, int>();
        node.SavedValues.ForEach(value => aaa.Add(value.Name, value.Value));
        int amount = Math.Max(0, consequence.Expression.Calculate(aaa).result);

        int moduleIndex = modules.FindIndex(module => module.Name == consequence.ModuleName);

        for (int i = 0; i != node.Children.Count; i++)
        {
            node.Children[i].Type = TreeNodeType.AND;
            node.Children[i].GenerateInstruction.Add(new ModuleGenerateInstruction(moduleIndex, amount));
        }
    }

    void GenerateSubModules(BuilderTreeNode tree, List<ModuleM1> modules)
    {
        if (tree.GenerateInstruction.Count > 0)
        {
            tree.Children.Add(new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<ItemsGroup>() { new ItemsGroup() {
                        Name = tree.Content[0].Name,
                        Items = new List<string>() { tree.Content[0].Items[0] }
                    } },
                ModuleList = tree.ModuleList
            });
            tree.Type = TreeNodeType.AND;
            tree.GenerateInstruction.ForEach(instruction =>
            {
                for (int i = 0; i != instruction.Amount; i++)
                {
                    tree.Children.Add(
                        Prebuild(modules, instruction.ModuleIndex, tree.ModuleList, i)
                    );
                }
            });
        }
        else
        {
            for (var childIndex = 0; childIndex != tree.Children.Count; childIndex++)
            {
                GenerateSubModules(tree.Children[childIndex], modules);
            }
        }
    }

    private void DivideTreeByModuleConstraint(BuilderTreeNode tree)
    {
        var subTrees = tree.Children[0].Children.Select(optionNode =>
        {
            var subTree = new BuilderTreeNode(tree);
            subTree.Content[0].Items = new List<string>() { optionNode.Content[0].Items[0] };
            subTree.SavedValues.Add(new IntegerItemsGroupValue()
            {
                Name = tree.Content[0].Name,
                Value = int.Parse(optionNode.Content[0].Items[0])
            });
            var leftSub = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<ItemsGroup>() {
                                        new ItemsGroup() {
                                            Name = tree.Content[0].Name,
                                            Type = tree.Content[0].Type,
                                            Items = new List<string>() { optionNode.Content[0].Items[0] }
                                        }
                                },

                ModuleList = tree.ModuleList
            };

            subTree.Children[0] = leftSub;
            leftSub.SavedValues.Add(new IntegerItemsGroupValue()
            {
                Name = tree.Content[0].Name,
                Value = int.Parse(optionNode.Content[0].Items[0])
            });

            leftSub.Children.Add(optionNode);

            for (int i = 1; i < subTree.Children.Count; i++)
            {
                subTree.Children[1].SavedValues.Add(new IntegerItemsGroupValue()
                {
                    Name = tree.Content[0].Name,
                    Value = int.Parse(optionNode.Content[0].Items[0])
                });
            }
            subTree.Type = TreeNodeType.AND;
            return subTree;
        });

        tree.Type = TreeNodeType.OR;
        tree.Children = subTrees.ToList();
    }

    private int GetTreeSize(BuilderTreeNode tree)
    {
        int accumulator = 0;
        foreach (var node in tree.Children)
        {
            accumulator += GetTreeSize(node);
        }
        return 1 + accumulator;
    }

    private void Optimize(BuilderTreeNode tree)
    {
        int treeSize = GetTreeSize(tree);

        if (tree.Children.Count == 1)
        {
            var temp = new BuilderTreeNode(tree.Children[0]);
            tree.Type = temp.Type;
            tree.Children = temp.Children.Select(c => new BuilderTreeNode(c)).ToList();
            tree.Content = temp.Content?.Select(c => new ItemsGroup(c)).ToList();
        }
        else if (tree.Type == TreeNodeType.AND)
        {
            var andChildren = tree.Children.Where(c => c.Type == TreeNodeType.AND).ToList();

            andChildren
                .ForEach(child =>
                {
                    child.Children.ToList().ForEach(subchild => tree.Children.Add(subchild));
                    tree.Children.Remove(child);
                });
        }
        else if (tree.Type == TreeNodeType.OR)
        {
            var orChildren = tree.Children.Where(c => c.Type == TreeNodeType.OR && c.Content == null).ToList();

            orChildren
                .ForEach(child =>
                {
                    child.Children.ToList().ForEach(subchild => tree.Children.Add(subchild));
                    tree.Children.Remove(child);
                });
        }

        for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
        {
            Optimize(tree.Children[childIndex]);
        }

        int newTreeSize = GetTreeSize(tree);

        if (newTreeSize < treeSize)
        {
            Optimize(tree);
        }
    }

    private void CombineContent(BuilderTreeNode targetNode, BuilderTreeNode sourceNode)
    {
        foreach (var sourceContent in sourceNode.Content)
        {
            if (targetNode.Content.FindIndex(content => content.Name == sourceContent.Name) == -1)
            {
                targetNode.Content.Add(new ItemsGroup() { Name = sourceContent.Name, Items = new List<string>() });
            }
            int contentIndex = targetNode.Content.FindIndex(content => content.Name == sourceContent.Name);
            foreach (var item in sourceContent.Items)
            {
                if (targetNode.Content[contentIndex].Items.FindIndex(i => i == item) == -1)
                {
                    targetNode.Content[contentIndex].Items.Add(item);
                }
            }
        }
    }

    private void PutContentInOrder(BuilderTreeNode node)
    {
        if (node.Children.Count != 0)
        {
            node.Content = new List<ItemsGroup>();

            foreach (var child in node.Children)
            {
                PutContentInOrder(child);
                CombineContent(node, child);
            }
        }
    }

    public BuilderTreeNode Prebuild(List<ModuleM1> modules, int moduleIndex, List<ModuleInstanceName>? moduleList = null, int moduleNumber = 0)
    {
        var tree = GenerateBasicTree(modules[moduleIndex], moduleList, moduleNumber);

        var featureConstraints = modules[moduleIndex].Constraints
            .Where(constraint => constraint.Consequence is ConstraintFeatureConsequence);
        var moduleConstraints = modules[moduleIndex].Constraints
            .Where(constraint => constraint.Consequence is ConstraintModuleConsequence);

        foreach (var constarint in featureConstraints)
        {
            ApplyFeatureConstraint(tree, constarint);
        }

        PutContentInOrder(tree);

        foreach (var constarint in moduleConstraints)
        {
            ApplyModuleConstraint(tree, constarint, modules);
        }

        PutContentInOrder(tree);

        GenerateSubModules(tree, modules);

        return tree;
    }

    public TreeNode[] Build(List<ModuleM1> modules, int moduleIndex)
    {
        var tree = Prebuild(modules, moduleIndex);

        //Optimize(tree);

        return ConvertBuilderTree(tree);
    }
}