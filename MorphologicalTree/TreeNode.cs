using System;
using System.Collections.Generic;

public enum TreeNodeType
{
    OR = 0,
    AND = 1
}

public class TreeNode
{
    public TreeNodeType type { get; set; }
    public List<int> children { get; set; }

    public string content { get; set; }
}