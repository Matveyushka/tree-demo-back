using System.Text;

namespace Structuralist.M1;

public class Builder
{
    private int limit = 5;

    private TreeNode[] ConvertBuilderTree(BuilderTreeNode sourceTree)
    {
        int nextNodeIndex = 1;
        var treeList = new List<TreeNode>();

        var queue = new Queue<BuilderTreeNode>();

        queue.Enqueue(sourceTree);

        while (queue.Count != 0)
        {
            var currentNode = queue.Dequeue();
            var newNode = new TreeNode()
            {
                Type = currentNode.Type
            };

            newNode.Content = currentNode.Content;

            foreach (var child in currentNode.Children)
            {
                queue.Enqueue(child);
                newNode.Children.Add(nextNodeIndex++);
            }

            treeList.Add(newNode);
        }

        return treeList.ToArray();
    }

    private BuilderTreeNode GenerateBasicTree(
        Module module,
        List<ModuleInstanceName>? moduleList = null,
        List<Feature>? restrictions = null,
        int moduleNumber = 0,
        string? alias = null)
    {
        if (moduleList is null)
        {
            moduleList = new List<ModuleInstanceName>();
        }

        restrictions = restrictions ?? new List<Feature>();

        var restrictedNames = restrictions
            .Select(feature => feature.Name);

        var restrictedFeatures = module
            .Features
            .Select(feature => restrictedNames.Contains(feature.Name)
                ? restrictions.First(f => f.Name == feature.Name)
                : feature)
            .ToList();

        var currentModuleInstanceName = new ModuleInstanceName
        {
            Name = alias ?? module.Name,
            Index = moduleNumber
        };

        var currentModuleList = moduleList.Select(l => l).Concat(new List<ModuleInstanceName>() { currentModuleInstanceName }).ToList();

        var tree = new BuilderTreeNode()
        {
            Type = TreeNodeType.AND,
            Children = new List<BuilderTreeNode>(),
            Content = restrictedFeatures.Select(feautre => new TreeNodeValue()
            {
                ModuleList = currentModuleList,
                Value = new Feature(feautre)
            }).ToList()
        };

        tree.Content.Add(new TreeNodeValue()
        {
            ModuleList = currentModuleList,
            Value = null
        });

        var pointer = tree;

        for (int featureIndex = 0; featureIndex != restrictedFeatures.Count; featureIndex++)
        {
            var choiceNode = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Children = restrictedFeatures[featureIndex].Values.Select(item => new BuilderTreeNode()
                {
                    Type = TreeNodeType.OR,
                    Children = new List<BuilderTreeNode>(),
                    Content = new List<TreeNodeValue>()
                    {
                        new TreeNodeValue() {
                            ModuleList = currentModuleList,
                            Value = new Feature() {
                                Name = restrictedFeatures[featureIndex].Name,
                                Values = new List<string>() { item },
                                Type = restrictedFeatures[featureIndex].Type
                            }
                        }
                    }
                }).ToList(),
            };
            pointer.Children.Add(choiceNode);
            if (featureIndex != restrictedFeatures.Count - 1)
            {
                var nextAndNode = new BuilderTreeNode()
                {
                    Type = TreeNodeType.AND,
                    Children = new List<BuilderTreeNode>(),
                    Content = restrictedFeatures.Where((group, index) => index > featureIndex)
                        .Select(feature => new TreeNodeValue()
                        {
                            ModuleList = currentModuleList,
                            Value = new Feature(feature)
                        }).ToList()
                };
                pointer.Children.Add(nextAndNode);
                pointer = nextAndNode;
            }
        }

        return tree;
    }

    private void DivideTreeByConstraintConditions(BuilderTreeNode tree, List<Feature> conditions)
    {
        var constraintOptions = conditions
            .Find(condition => condition.Name == tree.TopContent.Name)?
            .Values
            .ToList();

        var left = new BuilderTreeNode(tree);

        left.TopContent.Values = left.TopContent
            .Values
            .Where(item => constraintOptions?.FindIndex(option => option == item) != -1)
            .ToList();

        left.Children[0].Children = left.Children[0]
            .Children
            .Where(child => constraintOptions?.FindIndex(option => option == child.TopContent.Values[0]) != -1)
            .ToList();

        if (left.Children[0].Children.Count > 0)
        {
            var right = new BuilderTreeNode(tree);

            right.Children[0].Children = right.Children[0]
                .Children
                .Where(child => constraintOptions?.FindIndex(option => option == child.TopContent.Values[0]) == -1)
                .ToList();

            right.TopContent.Values = right.TopContent
                .Values
                .Where(item => constraintOptions?.FindIndex(option => option == item) == -1)
                .ToList();

            tree.Content.Clear();
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

    private void ApplyFeature(BuilderTreeNode tree, FeatureRule rule)
    {
        foreach (var consequence in rule.Consequences)
        {
            if (rule is not null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = rule
                        .Conditions
                        .FindIndex(condition => condition.Name == tree.TopContent.Name) != -1;

                    var topNodeFeaturePresentsInConqecuence = consequence
                        .Name == tree.TopContent.Name;

                    if (topNodeFeaturePresentsInCondition)
                    {
                        DivideTreeByConstraintConditions(tree, rule.Conditions);
                        if (tree.Children[0].Children.Count > 1)
                        {
                            ApplyFeature(tree.Children[0].Children[1], rule);
                        }
                    }
                    else if (tree.Children.Count > 1)
                    {
                        ApplyFeature(tree.Children[1], rule);
                    }
                    if (topNodeFeaturePresentsInConqecuence)
                    {
                        tree.TopContent.Values = tree.TopContent.Values.Where(item =>
                            consequence
                            .Values.FindIndex(option => option == item) != -1).ToList();

                        tree.Children[0].Children = tree.Children[0].Children.Where((child) =>
                            consequence
                            .Values.FindIndex(option => child.TopContent.Values[0] == option) != -1
                        ).ToList();
                    }
                }
                else
                {
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        ApplyFeature(tree.Children[childIndex], rule);
                    }
                }
            }
        }
    }

    private bool IsConstraintFullyApplied(
        string currentItemsGroupName,
        List<Feature> conditions,
        GenerateCommand command,
        BuilderTreeNode tree,
        List<Module> modules)
    {
        var currentModule = modules.Find(module => module.Name == tree.CurrentModule.Name);

        var lastConstraintItemsGroupIndex = 0;
        for (var i = 0; i != conditions.Count; i++)
        {
            var optionIndex = currentModule?
                .Features
                .FindIndex(x => x.Name == conditions[i].Name);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex ?? throw new NullReferenceException("optionIndex must not be null");
            }
        }

        var expressionVariables = command.QuantityExpression.GetVariables();
        for (var i = 0; i != expressionVariables.Count; i++)
        {
            var optionIndex = currentModule?
                .Features
                .FindIndex(x => x.Name == expressionVariables[i]);
            if (optionIndex > lastConstraintItemsGroupIndex)
            {
                lastConstraintItemsGroupIndex = optionIndex ?? throw new NullReferenceException("optionIndex must not be null");
            }
        }

        var currentItemsGroupIndex = currentModule?.Features.FindIndex(itemsGroup => itemsGroup.Name == currentItemsGroupName);

        return currentItemsGroupIndex >= lastConstraintItemsGroupIndex;
    }

    private void ApplyModuleConstraint(
        BuilderTreeNode tree,
        List<Feature> conditions,
        List<Feature> restrictions,
        GenerateCommand command,
        List<Module> modules)
    {
        if (tree.Type == TreeNodeType.AND)
        {
            var topNodeFeaturePresentsInCondition = conditions
                .FindIndex(condition => condition.Name == tree.TopContent.Name) != -1;

            var topNodeFeaturePresentsInConsequence = tree.Content.Count > 0
                ? command
                    .QuantityExpression
                    .IsVariableUsed(tree.TopContent.Name)
                : false;

            if (topNodeFeaturePresentsInCondition)
            {
                var condition = conditions
                    .First(condition => condition.Name == tree.TopContent.Name);

                var conditionIsFullfilled = (tree.TopContent.Values.All(conditionOption => condition.Values.Contains(conditionOption)));

                if (conditionIsFullfilled && topNodeFeaturePresentsInConsequence)
                {
                    DivideTreeByModuleConstraint(tree);
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        if (tree.Children[childIndex].SavedValues.Count >= command.QuantityExpression.GetVariablesQuantity() &&
                            IsConstraintFullyApplied(tree.TopContent.Name, conditions, command, tree, modules))
                        {
                            MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, restrictions, command);
                        }
                        else if (tree.Children[childIndex].Children.Count > 1)
                        {
                            ApplyModuleConstraint(tree.Children[childIndex].Children[1], conditions, restrictions, command, modules);
                        }
                    }
                }
                else if (conditionIsFullfilled)
                {
                    tree.Children[0].SavedValues = tree.SavedValues;
                    if (tree.SavedValues.Count == command.QuantityExpression.GetVariablesQuantity() &&
                        IsConstraintFullyApplied(tree.TopContent.Name, conditions, command, tree, modules))
                    {
                        MarkToGenerateSubModules(tree.Children[0], modules, restrictions, command);
                    }
                    else
                    {
                        for (var i = 1; i < tree.Children.Count; i++)
                        {
                            ApplyModuleConstraint(tree.Children[i], conditions, restrictions, command, modules);
                        }
                    }
                }
                else if (topNodeFeaturePresentsInConsequence)
                {
                    DivideTreeByConstraintConditions(tree, conditions);
                    ApplyModuleConstraint(tree.Children[0], conditions, restrictions, command, modules);
                }
                else
                {
                    DivideTreeByConstraintConditions(tree, conditions);
                    if (tree.Children[0].Children.Count > 1 && tree.Type == TreeNodeType.AND)
                    {
                        ApplyModuleConstraint(tree.Children[0].Children[1], conditions, restrictions, command, modules);
                    }
                    else if (tree.Type == TreeNodeType.OR)
                    {
                        ApplyModuleConstraint(tree.Children[0], conditions, restrictions, command, modules);
                    }
                }
            }
            else if (topNodeFeaturePresentsInConsequence)
            {
                DivideTreeByModuleConstraint(tree);
                for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                {
                    if (tree.Children[childIndex].SavedValues.Count == command.QuantityExpression.GetVariablesQuantity() &&
                            IsConstraintFullyApplied(tree.TopContent.Name, conditions, command, tree, modules))
                    {
                        MarkToGenerateSubModules(tree.Children[childIndex].Children[0], modules, restrictions, command);
                    }
                    else
                    {
                        for (var i = 1; i < tree.Children.Count; i++)
                        {
                            ApplyModuleConstraint(tree.Children[i], conditions, restrictions, command, modules);
                        }
                    }
                }
            }
            else if (conditions.Count == 0 && command.QuantityExpression.GetVariablesQuantity() == 0)
            {
                MarkToGenerateSubModules(tree.Children[0], modules, restrictions, command);
            }
            else
            {
                for (var i = 1; i < tree.Children.Count; i++)
                {
                    ApplyModuleConstraint(tree.Children[i], conditions, restrictions, command, modules);
                }
            }
        }
        else
        {
            for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
            {
                ApplyModuleConstraint(tree.Children[childIndex], conditions, restrictions, command, modules);
            }
        }
    }

    void MarkToGenerateSubModules(BuilderTreeNode node, List<Module> modules, List<Feature> restrictions, GenerateCommand command)
    {
        var aaa = new Dictionary<string, int>();
        node.SavedValues.ToList().ForEach(saved => aaa.Add(saved.Key, saved.Value));
        int amount = Math.Max(0, command.QuantityExpression.Calculate(aaa));

        int moduleIndex = modules.FindIndex(module => module.Name == command.ModuleName);

        for (int i = 0; i != node.Children.Count; i++)
        {
            node.Children[i].Type = TreeNodeType.AND;

            node.Children[i].GenerateInstructions.Add(new ModuleGenerateInstruction(
                moduleIndex,
                amount,
                restrictions,
                command.Alias));
        }
    }

    void GenerateSubModules(BuilderTreeNode tree, List<Module> modules, int depth)
    {
        if (depth >= this.limit)
        {
            return;
        }

        if (tree.GenerateInstructions.Count > 0)
        {
            tree.Children.Add(new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<TreeNodeValue>() {
                    new TreeNodeValue() {
                        ModuleList = tree.Content[0].ModuleList,
                        Value = new Feature(tree.TopContent)
                    }
                }
            });
            tree.Type = TreeNodeType.AND;
            var submodulesIndexes = new Dictionary<int, int>();
            tree.GenerateInstructions.ForEach(instruction =>
            {
                var submoduleIndexBegin = submodulesIndexes.ContainsKey(instruction.ModuleIndex)
                    ? submodulesIndexes[instruction.ModuleIndex]
                    : 0;
                if (depth != this.limit - 1)
                {
                    for (int i = 0; i != instruction.Amount; i++)
                    {
                        tree.Children.Add(
                            Prebuild(
                                modules,
                                instruction.ModuleIndex,
                                tree.Content[0].ModuleList,
                                instruction.Restrictions,
                                submoduleIndexBegin + i,
                                instruction.Alias,
                                depth + 1)
                        );
                    }
                }
                if (submoduleIndexBegin == 0)
                {
                    submodulesIndexes.Add(instruction.ModuleIndex, instruction.Amount);
                }
                else
                {
                    submodulesIndexes[instruction.ModuleIndex] += instruction.Amount;
                }
            });
        }
        else
        {
            for (var childIndex = 0; childIndex != tree.Children.Count; childIndex++)
            {
                GenerateSubModules(tree.Children[childIndex], modules, depth);
            }
        }
    }

    private void DivideTreeByModuleConstraint(BuilderTreeNode tree)
    {
        var subTrees = tree.Children[0].Children.Select(optionNode =>
        {
            var subTree = new BuilderTreeNode(tree);
            subTree.TopContent.Values = new List<string>() { optionNode.TopContent.Values[0] };
            if (subTree.SavedValues.ContainsKey(tree.TopContent.Name) == false)
            {
                subTree.SavedValues.Add(
                    tree.TopContent.Name,
                    int.Parse(optionNode.TopContent.Values[0])
                );
            }
            var leftSub = new BuilderTreeNode()
            {
                Type = TreeNodeType.OR,
                Content = new List<TreeNodeValue>() {
                    new TreeNodeValue() {
                        ModuleList = new List<ModuleInstanceName>() { tree.CurrentModule },
                        Value = new Feature(tree.TopContent)
                    }
                }
            };

            subTree.Children[0] = leftSub;
            if (leftSub.SavedValues.ContainsKey(tree.TopContent.Name) == false)
            {
                leftSub.SavedValues.Add(
                    tree.TopContent.Name,
                     int.Parse(optionNode.TopContent.Values[0])
                );
            }

            leftSub.Children.Add(optionNode);

            for (int i = 1; i < subTree.Children.Count; i++)
            {
                if (subTree.Children[1].SavedValues.ContainsKey(tree.TopContent.Name) == false)
                {
                    subTree.Children[1].SavedValues.Add(
                        tree.TopContent.Name,
                        int.Parse(optionNode.TopContent.Values[0])
                    );
                }
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
            tree.Children = new List<BuilderTreeNode>(temp.Children);
            tree.Content = temp.Content.Select(nodeValue => new TreeNodeValue(nodeValue)).ToList();
        }
        else if (tree.Type == TreeNodeType.AND)
        {
            var andChildren = tree.Children.Where(c => c.Type == TreeNodeType.AND).ToList();

            andChildren
                .ForEach(child =>
                {
                    if (child.Children.Count != 0)
                    {
                        child.Children.ToList().ForEach(subchild => tree.Children.Add(subchild));
                        tree.Children.Remove(child);
                    }
                    else
                    {
                        child.Type = TreeNodeType.OR;
                    }
                });

            var allChildrenAreLeafs = tree
                .Children
                .Aggregate<BuilderTreeNode, bool>(true, (result, child) => result && child.IsLeaf) &&
                tree.Children.Count > 0;

            if (allChildrenAreLeafs)
            {
                tree.Type = TreeNodeType.OR;
                tree.Children.ForEach(child => CombineContent(tree, child));
                tree.Children.Clear();
            }
        }
        else if (tree.Type == TreeNodeType.OR)
        {
            var orChildren = tree.Children.Where(c => c.Type == TreeNodeType.OR && c.Content.Count == 0).ToList();

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
            var sameValue = targetNode
                .Content
                .FirstOrDefault(nodeValue => nodeValue.SameModuleList(sourceContent) && (nodeValue.Value?.Name == sourceContent.Value?.Name));

            if (sameValue is default(TreeNodeValue))
            {
                targetNode.Content.Add(new TreeNodeValue()
                {
                    ModuleList = sourceContent.ModuleList.Select(instanceName => new ModuleInstanceName(instanceName)).ToList(),
                    Value = sourceContent.Value is not null ? new Feature(sourceContent.Value) : null
                });
            }
            else
            {
                if (sourceContent.Value is not null)
                {
                    foreach (var item in sourceContent.Value.Values)
                    {
                        if (sameValue.Value?.Values.FindIndex(i => i == item) == -1)
                        {
                            sameValue.Value.Values.Add(item);
                        }
                    }
                }
            }
        }
    }

    private void PutContentInOrder(BuilderTreeNode node)
    {
        if (node.Children.Count != 0)
        {
            node.Content = new List<TreeNodeValue>();

            foreach (var child in node.Children)
            {
                PutContentInOrder(child);
                CombineContent(node, child);
            }
        }
    }

    public BuilderTreeNode Prebuild(
        List<Module> modules,
        int moduleIndex,
        List<ModuleInstanceName>? moduleList = null,
        List<Feature>? restrictions = null,
        int moduleNumber = 0,
        string? alias = null,
        int depth = 0)
    {
        var tree = GenerateBasicTree(modules[moduleIndex], moduleList, restrictions, moduleNumber, alias);

        var featureRules = modules[moduleIndex].FeatureRules;
        var moduleRules = modules[moduleIndex].GenerateRules;

        foreach (var rule in featureRules)
        {
            ApplyFeature(tree, rule);
        }

        PutContentInOrder(tree);

        foreach (var rule in moduleRules)
        {
            foreach (var command in rule.Command)
            {
                ApplyModuleConstraint(tree, rule.Conditions, rule.Restrictions, command, modules);
            }
        }

        PutContentInOrder(tree);

        GenerateSubModules(tree, modules, depth);

        return tree;
    }

    public TreeNode[] Build(M1Model model)
    {
        this.limit = model.Create.Limit;

        var buildedModule = model.Modules.First(m => m.Name == model.Create.ModuleName);

        var tree = Prebuild(model.Modules, model.Modules.IndexOf(buildedModule));

        Optimize(tree);

        var result = ConvertBuilderTree(tree);

        return result;
    }
}