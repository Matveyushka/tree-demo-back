using System.Text;

namespace Structuralist.M1;

public class Builder
{
    private int limit = 5;

    private void ApplyValidOptions(
        BuilderTreeNode tree,
        List<string> constraintOptions,
        bool included)
    {
        tree.Children[0].Children = tree.Children[0]
            .Children
            .Where(child => (constraintOptions.FindIndex(option => option == child.TopContent.Values[0]) != -1) == included)
            .ToList();

        tree.TopContent.Values = tree.TopContent
            .Values
            .Where(item => (constraintOptions.FindIndex(option => option == item) != -1) == included)
            .ToList();
    }

    private BuilderTreeNode BuildSubtreeByConstraint(
        BuilderTreeNode tree,
        List<string> constraintOptions,
        bool included)
    {
        var subtree = new BuilderTreeNode(tree);

        ApplyValidOptions(subtree, constraintOptions, included);

        return subtree;
    }

    private void DivideTreeByConstraintConditions(BuilderTreeNode tree, List<Feature> conditions)
    {
        var constraintOptions = conditions
            .Find(condition => condition.Name == tree.TopContent.Name)?
            .Values
            .ToList()
            ?? new List<string>();

        var left = BuildSubtreeByConstraint(tree, constraintOptions, true);

        if (left.Children[0].Children.Count > 0)
        {
            var right = BuildSubtreeByConstraint(tree, constraintOptions, false);

            tree.Content.Clear();
            tree.Type = TreeNodeType.OR;
            tree.Children = new List<BuilderTreeNode>() { left };

            if (right.Children[0].Children.Count > 0)
            {
                tree.Children.Add(right);
            }
        }
    }

    private void ApplyFeature(BuilderTreeNode tree, FeatureRule rule)
    {
        if (tree.Type == TreeNodeType.AND)
        {
            if (rule.HasTopTreeFeatureInConditions(tree))
            {
                DivideTreeByConstraintConditions(tree, rule.Conditions);
                if (tree.Children[0].Children.Count > 1)
                {
                    ApplyFeature(tree.Children[0].Children[1], rule);
                }
            }
            else
            {
                if (rule.HasTopTreeFeatureInConsequence(tree))
                {
                    var suitableConsequence = rule
                        .Consequences
                        .First(consequence => consequence.Name == tree.TopContent.Name);

                    ApplyValidOptions(tree, suitableConsequence.Values, true);
                }
                if (tree.Children.Count > 1)
                {
                    ApplyFeature(tree.Children[1], rule);
                }
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

    private int GetDeepestFeatureIndex(
        Module currentModule,
        List<string> expressionVariables)
    {
        int deepestConstraintFeatureIndex = 0;

        for (var i = 0; i != expressionVariables.Count; i++)
        {
            var featureIndex = currentModule
                .Features
                .FindIndex(x => x.Name == expressionVariables[i]);
            if (featureIndex > deepestConstraintFeatureIndex)
            {
                deepestConstraintFeatureIndex = featureIndex;
            }
        }

        return deepestConstraintFeatureIndex;
    }

    private bool IsTreeDepthCoveringModuleConstraint(
        string currentFeatureName,
        List<Feature> conditions,
        GenerateCommand command,
        BuilderTreeNode tree,
        List<Module> modules)
    {
        var currentModule = modules.Find(module => module.Name == tree.CurrentModule.Name);

        if (currentModule is null)
        {
            throw new Exception($"Cannot find module named {tree.CurrentModule.Name}");
        }

        var currentFeatureIndex = currentModule.Features.FindIndex(Feature => Feature.Name == currentFeatureName);

        var conditionFeatures = conditions.Select(condition => condition.Name).ToList();

        var expressionVariables = command.QuantityExpression.GetVariables();

        return currentFeatureIndex >= GetDeepestFeatureIndex(currentModule, conditionFeatures)
            && currentFeatureIndex >= GetDeepestFeatureIndex(currentModule, expressionVariables);
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
            var matchedCondition = conditions
                .FirstOrDefault(condition => condition.Name == tree.TopContent.Name);

            var conditionIsFullfilled = matchedCondition is not null && tree
                .TopContent
                .Values
                .All(conditionOption => matchedCondition.Values.Contains(conditionOption));

            var topNodeFeaturePresentsInConsequence = command
                .QuantityExpression
                .IsVariableUsed(tree.TopContent.Name);

            if (conditions.Count == 0 && command.QuantityExpression.GetVariablesQuantity() == 0)
            {
                MarkToGenerateSubModules(tree.Children[0], modules, restrictions, command);
            }
            else if ((conditionIsFullfilled || matchedCondition is null)
                    && tree.SavedValues.Count == command.QuantityExpression.GetVariablesQuantity()
                    && IsTreeDepthCoveringModuleConstraint(tree.TopContent.Name, conditions, command, tree, modules))
            {
                tree.Children[0].SavedValues = new Dictionary<string, int>(tree.SavedValues);
                MarkToGenerateSubModules(tree.Children[0], modules, restrictions, command);
            }
            else if (conditionIsFullfilled == false && matchedCondition is not null)
            {
                DivideTreeByConstraintConditions(tree, conditions);
                tree.Children[0].SavedValues = new Dictionary<string, int>(tree.SavedValues);
                ApplyModuleConstraint(tree.Children[0], conditions, restrictions, command, modules);
            }
            else if (topNodeFeaturePresentsInConsequence)
            {
                if (tree.SavedValues.ContainsKey(tree.TopContent.Name) == false)
                {
                    DivideTreeByModuleConstraint(tree);
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        ApplyModuleConstraint(tree.Children[childIndex], conditions, restrictions, command, modules);
                    }
                }
                else if (tree.Children.Count >= 2)
                {
                    tree.Children[1].SavedValues = new Dictionary<string, int>(tree.SavedValues);
                    ApplyModuleConstraint(tree.Children[1], conditions, restrictions, command, modules);
                }
            }
            else
            {
                if (tree.Children.Count >= 2)
                {
                    tree.Children[1].SavedValues = new Dictionary<string, int>(tree.SavedValues);
                    ApplyModuleConstraint(tree.Children[1], conditions, restrictions, command, modules);
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
        int amount = Math.Max(0, command.QuantityExpression.Calculate(node.SavedValues));

        int moduleIndex = modules.FindIndex(module => module.Name == command.ModuleName);

        for (int i = 0; i != node.Children.Count; i++)
        {
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
                },
                SavedValues = new Dictionary<string, int>(subTree.SavedValues)
            };

            subTree.Children[0] = leftSub;
            if (leftSub.SavedValues.ContainsKey(tree.TopContent.Name) == false)
            {
                leftSub.SavedValues.Add(
                    tree.TopContent.Name,
                     int.Parse(optionNode.TopContent.Values[0])
                );
            }

            optionNode.SavedValues = new Dictionary<string, int>(subTree.SavedValues);
            leftSub.Children.Add(optionNode);

            for (int i = 1; i < subTree.Children.Count; i++)
            {
                subTree.Children[i].SavedValues = new Dictionary<string, int>(subTree.SavedValues);
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
        var basicTreeCreator = new BasicTreeCreator(
            modules[moduleIndex],
            moduleList,
            restrictions,
            moduleNumber,
            alias);

        var tree = basicTreeCreator.CreateBasicTree();

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
        this.limit = model
            .CreateCommand
            .Limit;

        var moduleBeingBuilded = model
            .Modules
            .First(m => m.Name == model.CreateCommand.ModuleName);

        var tree = Prebuild(model.Modules, model.Modules.IndexOf(moduleBeingBuilded));

        Optimize(tree);

        var resultTree = tree.ToOutputTree();

        return resultTree;
    }
}