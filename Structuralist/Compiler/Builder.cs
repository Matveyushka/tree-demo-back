using System.Collections.Generic;
using System.Linq;

namespace tree_demo_back
{
    public class BuilderTreeNode
    {
        public TreeNodeType Type { get; set; }
        public List<BuilderTreeNode> Children { get; set; }
        public List<ItemsGroup> Content { get; set; }

        public BuilderTreeNode() { }

        public BuilderTreeNode(BuilderTreeNode source)
        {
            Type = source.Type;
            Content = source.Content?.Select(c => new ItemsGroup(c)).ToList();
            Children = source.Children?.Select(c => new BuilderTreeNode(c)).ToList();
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
                if (currentNode.Children.Count == 0 && currentNode.Content != null && currentNode.Content.Count == 1)
                {
                    treeList.Last().content = $"{currentNode.Content[0].Name}: {currentNode.Content[0].Items[0]}";
                }
                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue(child);
                    treeList.Last().children.Add(nextNodeIndex++);
                }
            }

            return treeList.ToArray();
        }

        private BuilderTreeNode GenerateBasicTree(ModuleM1 module)
        {
            var tree = new BuilderTreeNode()
            {
                Type = TreeNodeType.AND,
                Children = new List<BuilderTreeNode>(),
                Content = module.ClassificationFeatures
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
                        }
                    }).ToList()
                };
                pointer.Children.Add(choiceNode);
                if (featureIndex != module.ClassificationFeatures.Count - 1)
                {
                    var nextAndNode = new BuilderTreeNode()
                    {
                        Type = TreeNodeType.AND,
                        Children = new List<BuilderTreeNode>(),
                        Content = module.ClassificationFeatures.Where((group, index) => index > featureIndex).ToList()
                    };
                    pointer.Children.Add(nextAndNode);
                    pointer = nextAndNode;
                }
            }

            return tree;
        }

        /*private bool ItemsIntersect(ItemsGroup group1, ItemsGroup group2)
        {
            var items1 = group1.Items;
            var items2 = group2.Items;

            return items1.Intersect(items2).Count() > 0;
        }*/

        private void ApplyConstraint(BuilderTreeNode tree, Constraint constraint)
        {
            if (constraint != null)
            {
                if (tree.Type == TreeNodeType.AND)
                {
                    var topNodeFeaturePresentsInCondition = constraint
                        .Сonditions
                        .FindIndex(condition => condition.СlassificationFeatureName == tree.Content[0].Name) != -1;

                    var topNodeFeaturePresentsInConqecuence = constraint
                        .Сonsequence.СlassificationFeatureName == tree.Content[0].Name;

                    if (topNodeFeaturePresentsInCondition)
                    {
                        var constraintOptions = constraint
                            .Сonditions
                            .Find(condition => condition.СlassificationFeatureName == tree.Content[0].Name)
                            .СonditionOptions
                            .ToList();

                        var left = new BuilderTreeNode(tree);

                        left.Content[0].Items = left.Content[0].Items.Where(item => constraintOptions.FindIndex(option => option == item) != -1).ToList();

                        left.Children[0].Children = left.Children[0].Children
                            .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Items[0]) != -1).ToList();

                        if (left.Children[0].Children.Count > 0)
                        {
                            var right = new BuilderTreeNode(tree);

                            right.Children[0].Children = right.Children[0].Children
                                .Where(child => constraintOptions.FindIndex(option => option == child.Content[0].Items[0]) == -1).ToList();

                            right.Content[0].Items = right.Content[0].Items.Where(item => constraintOptions.FindIndex(option => option == item) == -1).ToList();

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

                            ApplyConstraint(left.Children[1], constraint);
                        }
                    }
                    else if (tree.Children.Count > 1)
                    {
                        ApplyConstraint(tree.Children[1], constraint);
                    }
                    if (topNodeFeaturePresentsInConqecuence)
                    {
                        tree.Children[0].Children = tree.Children[0].Children.Where((child) =>
                            constraint.Сonsequence.ValidOptions.FindIndex(option => child.Content[0].Items[0] == option) != -1
                        ).ToList();
                    }
                }
                else
                {
                    for (int childIndex = 0; childIndex != tree.Children.Count; childIndex++)
                    {
                        ApplyConstraint(tree.Children[childIndex], constraint);
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

        public TreeNode[] Build(List<ModuleM1> modules, int moduleIndex)
        {
            var tree = GenerateBasicTree(modules[moduleIndex]);

            foreach (var constarint in modules[moduleIndex].Constraints)
            {
                ApplyConstraint(tree, constarint);
            }

            Optimize(tree);

            return ConvertBuilderTree(tree);
        }
    }
}
