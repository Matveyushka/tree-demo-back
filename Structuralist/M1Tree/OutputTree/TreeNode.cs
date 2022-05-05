namespace Structuralist.M1Tree;

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