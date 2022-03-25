namespace Structuralist.M1;

public enum TreeNodeType
{
    OR = 0,
    AND = 1
}

public class TreeNode
{
    public TreeNodeType type { get; set; }
    public List<int> children { get; set; } = new List<int>();
    public string content { get; set; } = null!;
    public string moduleList { get; set; } = null!;
}

public static class TreeNodeExtensions
{
    private static bool NodeNeedsGene(TreeNode node) => node.type == TreeNodeType.OR && node.children.Count > 0;

    private static (int index, int size) GetGeneFromNode(int index, TreeNode node) => (
        index,
        node.children.Count
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