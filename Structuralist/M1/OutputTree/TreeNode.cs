namespace Structuralist.M1;

public enum TreeNodeType
{
    OR = 0,
    AND = 1
}

public class TreeNode
{
    public TreeNodeType Type { get; set; }
    public List<int> Children { get; set; } = new List<int>();
    public List<TreeNodeValue> Content { get; set; } = new List<TreeNodeValue>();
    public Dictionary<string, int> SavedValues { get; set; } = new Dictionary<string, int>();
}

public static class TreeNodeExtensions
{
    private static bool NodeNeedsGene(TreeNode node) => node.Type == TreeNodeType.OR && node.Children.Count > 0;

    private static (int index, int size) GetGeneFromNode(int index, TreeNode node) => (
        index,
        node.Children.Count
    );

    public static Dictionary<int, int> GetGenotypeStructure(this TreeNode[] tree)
    {
        var genotype = new Dictionary<int, int>();
        for (var i = 0; i != tree.Length; i++)
        {
            if (NodeNeedsGene(tree[i]))
            {
                var geneStructure = GetGeneFromNode(i, tree[i]);
                genotype.Add(geneStructure.index, geneStructure.size);
            }
        }
        return genotype;
    }
}