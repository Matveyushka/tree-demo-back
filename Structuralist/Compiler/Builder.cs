using System.Collections.Generic;
using System.Linq;

namespace tree_demo_back
{
    public class BuilderTreeNode
    {
        public TreeNodeType Type { get; set; }
        public List<BuilderTreeNode> Children { get; set; }
        public List<ItemsGroup> Content { get; set; }
        public List<string> ModuleList { get; set; }

        public BuilderTreeNode() { }

        public BuilderTreeNode(BuilderTreeNode source)
        {
            Type = source.Type;
            Content = source.Content?.Select(c => new ItemsGroup(c)).ToList();
            Children = source.Children?.Select(c => new BuilderTreeNode(c)).ToList();
            ModuleList = source.ModuleList?.Select(c => c).ToList();
        }
    }

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
                    var currentModuleName = "";
                    foreach (var moduleName in currentNode.ModuleList)
                    {
                        currentModuleName += moduleName + ".";
                    }
                    var content = "";
                    foreach (var current in currentNode.Content)
                    {
                        content += currentModuleName + current.Name + ": ";
                        foreach (var item in current.Items)
                        {
                            content += item + " ";
                        }
                        content += "; ";
                    }
                    treeList.Last().content = content;
                }

                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue(child);
                    treeList.Last().children.Add(nextNodeIndex++);
                }
            }

            return treeList.ToArray();
        }

        private bool IsCurrentModule(BuilderTreeNode node, List<string> moduleList)
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
        }

        private BuilderTreeNode GenerateBasicTree(ModuleM1 module, List<string> moduleList = null)
        {
            var tree = new BuilderTreeNode()
            {
                Type = TreeNodeType.AND,
                Children = new List<BuilderTreeNode>(),
                Content = module.ClassificationFeatures,
                ModuleList = (moduleList ?? new List<string>()).Concat(new List<string>() { module.Name }).ToList()
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
                        ModuleList = tree.ModuleList.Select(c => c).ToList()
                    }).ToList(),
                    ModuleList = tree.ModuleList.Select(c => c).ToList()
                };
                pointer.Children.Add(choiceNode);
                if (featureIndex != module.ClassificationFeatures.Count - 1)
                {
                    var nextAndNode = new BuilderTreeNode()
                    {
                        Type = TreeNodeType.AND,
                        Children = new List<BuilderTreeNode>(),
                        Content = module.ClassificationFeatures.Where((group, index) => index > featureIndex).ToList(),
                        ModuleList = tree.ModuleList.Select(c => c).ToList()
                    };
                    pointer.Children.Add(nextAndNode);
                    pointer = nextAndNode;
                }
            }

            return tree;
        }

        private void ApplyFeatureConstraint(BuilderTreeNode tree, Constraint constraint)
        {
            if (constraint != null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = constraint
                        .Сonditions
                        .FindIndex(condition => condition.СlassificationFeatureName == tree.Content[0].Name) != -1;

                    bool topNodeFeaturePresentsInConqecuence = false;

                    topNodeFeaturePresentsInConqecuence = (constraint
                       .Consequence as ConstraintFeatureConsequence)
                       .СlassificationFeatureName == tree.Content[0].Name;

                    if (topNodeFeaturePresentsInCondition)
                    {
                        var constraintOptions = constraint
                            .Сonditions
                            .Find(condition => condition.СlassificationFeatureName == tree.Content[0].Name)
                            .СonditionOptions
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

                            ApplyFeatureConstraint(left.Children[1], constraint);
                        }
                    }
                    else if (tree.Children.Count > 1)
                    {
                        ApplyFeatureConstraint(tree.Children[1], constraint);
                    }
                    if (topNodeFeaturePresentsInConqecuence)
                    {
                        tree.Content[0].Items = tree.Content[0].Items.Where(item =>
                        (constraint.Consequence as ConstraintFeatureConsequence)
                            .ValidOptions.FindIndex(option => option == item) != -1).ToList();

                        tree.Children[0].Children = tree.Children[0].Children.Where((child) =>
                            (constraint.Consequence as ConstraintFeatureConsequence)
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

        private void ApplyModuleConstraint(BuilderTreeNode tree, Constraint constraint, List<ModuleM1> modules)
        {
            if (constraint != null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = constraint
                        .Сonditions
                        .FindIndex(condition => condition.СlassificationFeatureName == tree.Content[0].Name) != -1;

                    if (topNodeFeaturePresentsInCondition)
                    {
                        var constraintOptions = constraint
                            .Сonditions
                            .Find(condition => condition.СlassificationFeatureName == tree.Content[0].Name)
                            .СonditionOptions
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

                            if (left.Content[0].Name == constraint.Сonditions.Last().СlassificationFeatureName)
                            {
                                var moduleIndex = modules
                                    .FindIndex(module => module.Name == (constraint.Consequence as ConstraintModuleConsequence).ModuleName);
                                left.Children.Add(Prebuild(modules, moduleIndex, tree.ModuleList));

                            }
                            else
                            {
                                ApplyModuleConstraint(left.Children[1], constraint, modules);
                            }
                        }
                    }
                    else if (tree.Children.Count > 1)
                    {
                        ApplyModuleConstraint(tree.Children[1], constraint, modules);
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
                if (node.Content != null)
                {
                    node.Content = new List<ItemsGroup>() { node.Content.First() } ;
                }
                else
                {
                    node.Content = new List<ItemsGroup>();
                }
                
                foreach (var child in node.Children)
                {
                    PutContentInOrder(child);
                    CombineContent(node, child); 
                }
            }
        }

        public BuilderTreeNode Prebuild(List<ModuleM1> modules, int moduleIndex, List<string> moduleList = null)
        {
            var tree = GenerateBasicTree(modules[moduleIndex], moduleList);

            var featureConstraints = modules[moduleIndex].Constraints
                .Where(constraint => constraint.Consequence is ConstraintFeatureConsequence);
            var moduleConstraints = modules[moduleIndex].Constraints
                .Where(constraint => constraint.Consequence is ConstraintModuleConsequence);

            foreach (var constarint in featureConstraints)
            {
                ApplyFeatureConstraint(tree, constarint);
            }
            foreach (var constarint in moduleConstraints)
            {
                ApplyModuleConstraint(tree, constarint, modules);
            }

            PutContentInOrder(tree);

            return tree;
        }

        public TreeNode[] Build(List<ModuleM1> modules, int moduleIndex)
        {
            var tree = Prebuild(modules, moduleIndex);

            Optimize(tree);

            return ConvertBuilderTree(tree);
        }
    }
}
