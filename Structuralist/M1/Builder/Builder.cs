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
                    foreach (var item in current.Values)
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
    private BuilderTreeNode GenerateBasicTree(Module module, List<ModuleInstanceName>? moduleList = null, int moduleNumber = 0)
    {
        var tree = new BuilderTreeNode()
        {
            Type = TreeNodeType.AND,
            Children = new List<BuilderTreeNode>(),
            Content = module.Features,
            ModuleList = (moduleList ?? new List<ModuleInstanceName>())
                .Concat(new List<ModuleInstanceName>() { new ModuleInstanceName {
                        name = module.Name,
                        index = moduleNumber
                    } }).ToList()
        };

        var pointer = tree;

        for (int featureIndex = 0; featureIndex != module.Features.Count; featureIndex++)
        {
            var choiceNode = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Children = module.Features[featureIndex].Values.Select(item => new BuilderTreeNode()
                {
                    Type = TreeNodeType.OR,
                    Children = new List<BuilderTreeNode>(),
                    Content = new List<Feature>() {
                            new Feature() {
                                Name = module.Features[featureIndex].Name,
                                Values = new List<string>() { item }
                            }
                        },
                    ModuleList = tree.ModuleList
                }).ToList(),
                ModuleList = tree.ModuleList
            };
            pointer.Children.Add(choiceNode);
            if (featureIndex != module.Features.Count - 1)
            {
                var nextAndNode = new BuilderTreeNode()
                {
                    Type = TreeNodeType.AND,
                    Children = new List<BuilderTreeNode>(),
                    Content = module.Features.Where((group, index) => index > featureIndex).ToList(),
                    ModuleList = tree.ModuleList
                };
                pointer.Children.Add(nextAndNode);
                pointer = nextAndNode;
            }
        }

        return tree;
    }

    private void DivideTreeByConstraintConditions(BuilderTreeNode tree, List<FeatureConstraint> conditions)
    {
        var constraintOptions = conditions
            .Find(condition => condition.FeatureName == tree.Content[0].Name)
            .ValidValues
            .ToList();

        var left = new BuilderTreeNode(tree);

        left.Content[0].Values = left.Content[0]
            .Values
            .Where(item => constraintOptions.FindIndex(option => option == item) != -1)
            .ToList();

        left.Children[0].Children = left.Children[0]
            .Children
            .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Values[0]) != -1)
            .ToList();

        if (left.Children[0].Children.Count > 0)
        {
            var right = new BuilderTreeNode(tree);

            right.Children[0].Children = right.Children[0]
                .Children
                .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Values[0]) == -1)
                .ToList();

            right.Content[0].Values = right.Content[0]
                .Values
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

    private void ApplyFeatureConstraint(BuilderTreeNode tree, FeatureRule rule)
    {
        foreach (var consequence in rule.Consequences)
        {
            if (rule is not null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = rule
                        .Conditions
                        .FindIndex(condition => condition.FeatureName == tree.Content[0].Name) != -1;

                    var topNodeFeaturePresentsInConqecuence = consequence
                        .FeatureName == tree.Content[0].Name;

                    if (topNodeFeaturePresentsInCondition)
                    {
                        DivideTreeByConstraintConditions(tree, rule.Conditions);
                        if (tree.Children[0].Children.Count > 1)
                        {
                            ApplyFeatureConstraint(tree.Children[0].Children[1], rule);
                        }
                    }
                    else if (tree.Children.Count > 1)
                    {
                        ApplyFeatureConstraint(tree.Children[1], rule);
                    }
                    if (topNodeFeaturePresentsInConqecuence)
                    {
                        tree.Content[0].Values = tree.Content[0].Values.Where(item =>
                            consequence
                            .ValidValues.FindIndex(option => option == item) != -1).ToList();

                        tree.Children[0].Children = tree.Children[0].Children.Where((child) =>
                            consequence
                            .ValidValues.FindIndex(option => child.Content[0].Values[0] == option) != -1
                        ).ToList();
                    }
                }
                else
                {
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        ApplyFeatureConstraint(tree.Children[childIndex], rule);
                    }
                }
            }
        }
    }

    private bool IsConstraintFullyApplied(
        string currentItemsGroupName,
        List<FeatureConstraint> conditions,
        GenerateCommand command,
        BuilderTreeNode tree,
        List<Module> modules)
    {
        var currentModule = modules.Find(module => module.Name == tree.ModuleList.Last().name);

        var lastConstraintItemsGroupIndex = 0;
        for (var i = 0; i != conditions.Count; i++)
        {
            var optionIndex = currentModule
                .Features
                .FindIndex(x => x.Name == conditions[i].FeatureName);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex;
            }
        }

        var expressionVariables = command.QuantityExpression.GetVariables();
        for (var i = 0; i != expressionVariables.Count; i++)
        {
            var optionIndex = currentModule
                .Features
                .FindIndex(x => x.Name == expressionVariables[i]);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex;
            }
        }

        var currentItemsGroupIndex = currentModule.Features.FindIndex(itemsGroup => itemsGroup.Name == currentItemsGroupName);

        return currentItemsGroupIndex >= lastConstraintItemsGroupIndex;
    }

    private void ApplyModuleConstraint(BuilderTreeNode tree, GenerateRule rule, List<Module> modules)
    {
        foreach (var command in rule.Command)
        {
            if (rule is not null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = rule
                        .Conditions
                        .FindIndex(condition => condition.FeatureName == tree.Content[0].Name) != -1;

                    var topNodeFeaturePresentsInConsequence = command
                        .QuantityExpression
                        .IsVariableUsed(tree.Content[0].Name);

                    if (topNodeFeaturePresentsInCondition)
                    {
                        var condition = rule
                            .Conditions
                            .First(condition => condition.FeatureName == tree.Content[0].Name);

                        var conditionIsFullfilled = (tree.Content[0].Values.All(conditionOption => condition.ValidValues.Contains(conditionOption)));

                        if (conditionIsFullfilled && topNodeFeaturePresentsInConsequence)
                        {
                            DivideTreeByModuleConstraint(tree);
                            for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                            {
                                if (tree.Children[childIndex].SavedValues.Count >= command.QuantityExpression.GetVariablesQuantity() &&
                                    IsConstraintFullyApplied(tree.Content[0].Name, rule.Conditions, command, tree, modules))
                                {
                                    MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, command);
                                }
                                else if (tree.Children[childIndex].Children.Count > 1)
                                {
                                    ApplyModuleConstraint(tree.Children[childIndex].Children[1], rule, modules);
                                }
                            }
                        }
                        else if (conditionIsFullfilled)
                        {
                            tree.Children[0].SavedValues = tree.SavedValues;
                            if (tree.SavedValues.Count == command.QuantityExpression.GetVariablesQuantity() &&
                                IsConstraintFullyApplied(tree.Content[0].Name, rule.Conditions, command, tree, modules))
                            {
                                MarkToGenerateSubModules(tree.Children[0], modules, command);
                            }
                            else
                            {
                                for (var i = 1; i < tree.Children.Count; i++)
                                {
                                    ApplyModuleConstraint(tree.Children[i], rule, modules);
                                }
                            }
                        }
                        else if (topNodeFeaturePresentsInConsequence)
                        {
                            DivideTreeByConstraintConditions(tree, rule.Conditions);
                            ApplyModuleConstraint(tree.Children[0], rule, modules);
                        }
                        else
                        {
                            DivideTreeByConstraintConditions(tree, rule.Conditions);
                            if (tree.Children[0].Children.Count > 1 && tree.Type == TreeNodeType.AND)
                            {
                                ApplyModuleConstraint(tree.Children[0].Children[1], rule, modules);
                            }
                            else if (tree.Type == TreeNodeType.OR)
                            {
                                ApplyModuleConstraint(tree.Children[0], rule, modules);
                            }
                        }
                    }
                    else if (topNodeFeaturePresentsInConsequence)
                    {
                        DivideTreeByModuleConstraint(tree);
                        for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                        {
                            if (tree.Children[childIndex].SavedValues.Count == command.QuantityExpression.GetVariablesQuantity() &&
                                    IsConstraintFullyApplied(tree.Content[0].Name, rule.Conditions, command, tree, modules))
                            {
                                MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, command);
                            }
                            else
                            {
                                for (var i = 1; i < tree.Children.Count; i++)
                                {
                                    ApplyModuleConstraint(tree.Children[i], rule, modules);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var i = 1; i < tree.Children.Count; i++)
                        {
                            ApplyModuleConstraint(tree.Children[i], rule, modules);
                        }
                    }
                }
                else
                {
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        ApplyModuleConstraint(tree.Children[childIndex], rule, modules);
                    }
                }
            }
        }
    }

    void MarkToGenerateSubModules(BuilderTreeNode node, List<Module> modules, GenerateCommand command)
    {
        var aaa = new Dictionary<string, int>();
        node.SavedValues.ForEach(value => aaa.Add(value.Name, value.Value));
        int amount = Math.Max(0, command.QuantityExpression.Calculate(aaa));

        int moduleIndex = modules.FindIndex(module => module.Name == command.ModuleName);

        for (int i = 0; i != node.Children.Count; i++)
        {
            node.Children[i].Type = TreeNodeType.AND;
            node.Children[i].GenerateInstruction.Add(new ModuleGenerateInstruction(moduleIndex, amount));
        }
    }

    void GenerateSubModules(BuilderTreeNode tree, List<Module> modules)
    {
        if (tree.GenerateInstruction.Count > 0)
        {
            tree.Children.Add(new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<Feature>() { new Feature() {
                        Name = tree.Content[0].Name,
                        Values = new List<string>() { tree.Content[0].Values[0] }
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
            subTree.Content[0].Values = new List<string>() { optionNode.Content[0].Values[0] };
            subTree.SavedValues.Add(new IntegerItemsGroupValue()
            {
                Name = tree.Content[0].Name,
                Value = int.Parse(optionNode.Content[0].Values[0])
            });
            var leftSub = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<Feature>() {
                                        new Feature() {
                                            Name = tree.Content[0].Name,
                                            Type = tree.Content[0].Type,
                                            Values = new List<string>() { optionNode.Content[0].Values[0] }
                                        }
                                },

                ModuleList = tree.ModuleList
            };

            subTree.Children[0] = leftSub;
            leftSub.SavedValues.Add(new IntegerItemsGroupValue()
            {
                Name = tree.Content[0].Name,
                Value = int.Parse(optionNode.Content[0].Values[0])
            });

            leftSub.Children.Add(optionNode);

            for (int i = 1; i < subTree.Children.Count; i++)
            {
                subTree.Children[1].SavedValues.Add(new IntegerItemsGroupValue()
                {
                    Name = tree.Content[0].Name,
                    Value = int.Parse(optionNode.Content[0].Values[0])
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
            tree.Content = temp.Content?.Select(c => new Feature() { Name = c.Name, Values = new List<string>(c.Values) }).ToList();
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
                targetNode.Content.Add(new Feature() { Name = sourceContent.Name, Values = new List<string>() });
            }
            int contentIndex = targetNode.Content.FindIndex(content => content.Name == sourceContent.Name);
            foreach (var item in sourceContent.Values)
            {
                if (targetNode.Content[contentIndex].Values.FindIndex(i => i == item) == -1)
                {
                    targetNode.Content[contentIndex].Values.Add(item);
                }
            }
        }
    }

    private void PutContentInOrder(BuilderTreeNode node)
    {
        if (node.Children.Count != 0)
        {
            node.Content = new List<Feature>();

            foreach (var child in node.Children)
            {
                PutContentInOrder(child);
                CombineContent(node, child);
            }
        }
    }

    public BuilderTreeNode Prebuild(List<Module> modules, int moduleIndex, List<ModuleInstanceName>? moduleList = null, int moduleNumber = 0)
    {
        var tree = GenerateBasicTree(modules[moduleIndex], moduleList, moduleNumber);

        var featureRules = modules[moduleIndex].FeatureRules;
        var moduleRules = modules[moduleIndex].GenerateRules;

        foreach (var rule in featureRules)
        {
            ApplyFeatureConstraint(tree, rule);
        }

        PutContentInOrder(tree);

        foreach (var rule in moduleRules)
        {
            ApplyModuleConstraint(tree, rule, modules);
        }

        PutContentInOrder(tree);

        GenerateSubModules(tree, modules);

        return tree;
    }

    public TreeNode[] Build(List<Module> modules, int moduleIndex)
    {
        var tree = Prebuild(modules, moduleIndex);

        Optimize(tree);

        return ConvertBuilderTree(tree);
    }
}