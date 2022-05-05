using Structuralist.M1;
using Structuralist.M1Tree;

namespace Genetic;

public class GenotypeStructure
{
    public Dictionary<int, int> TreeDimensions { get; private set; }
    public Dictionary<string, List<RealParameter>> ParametersDimensions { get; private set; }

    private static bool NodeNeedsGene(TreeNode node) => node.Type == TreeNodeType.OR && node.Children.Count > 0;

    public GenotypeStructure(TreeNode[] tree, M1Model m1Model)
    {
        this.TreeDimensions = new Dictionary<int, int>();
        this.ParametersDimensions = new Dictionary<string, List<RealParameter>>();

        for (var treeNodeIndex = 0; treeNodeIndex != tree.Length; treeNodeIndex++)
        {
            if (NodeNeedsGene(tree[treeNodeIndex]))
            {
                this.TreeDimensions.Add(treeNodeIndex, tree[treeNodeIndex].Children.Count);
            }
            for (var i = 0; i != tree[treeNodeIndex].Content.Count; i++)
            {
                var fullModuleName = tree[treeNodeIndex].Content[i].ModuleList.Aggregate(
                    "", (name, moduleInstanceName) => $"{name}{moduleInstanceName.Name}[{moduleInstanceName.Index}]"
                );
                if (this.ParametersDimensions.ContainsKey(fullModuleName) == false)
                {
                    this.ParametersDimensions.Add(
                        fullModuleName,
                        m1Model.Modules.First(module => module.Name == tree[treeNodeIndex].Content[i].ModuleList.Last().Name).Parameters
                    );
                }
            }
        }
    }
}