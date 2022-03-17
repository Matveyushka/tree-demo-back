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
}