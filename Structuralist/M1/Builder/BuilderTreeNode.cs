namespace Structuralist.M1;

public class BuilderTreeNode
{
    public TreeNodeType Type { get; set; }
    public List<BuilderTreeNode> Children { get; set; } = new List<BuilderTreeNode>();
    public Dictionary<string, int> SavedValues { get; set; } = new Dictionary<string, int>();
    public List<TreeNodeValue> Content { get; set; } = new List<TreeNodeValue>();
    public List<ModuleGenerateInstruction> GenerateInstructions { get; set; } = new List<ModuleGenerateInstruction>();

    public bool IsLeaf => this.Children.Count == 0 && this.Content.Count > 0;
    public Feature TopContent => this.Content[0].Value!;
    public ModuleInstanceName CurrentModule => this.Content[0].ModuleList.Last();

    public BuilderTreeNode()
    {
    }

    public BuilderTreeNode(BuilderTreeNode source)
    {
        if (source is null)
        {
            throw new NullReferenceException("Copy constructor argument of BuilderTreeNode must not be null");
        }
        Type = source.Type;
        Content = source.Content.Select(nodeValue => new TreeNodeValue(nodeValue)).ToList();
        Children = source.Children.Select(child => new BuilderTreeNode(child)).ToList();
        SavedValues = new Dictionary<string, int>();
        foreach (var sourceSavedValuesEntry in source.SavedValues)
        {
            SavedValues.Add(sourceSavedValuesEntry.Key, sourceSavedValuesEntry.Value);
        }
        GenerateInstructions = source.GenerateInstructions.Select(instruction => new ModuleGenerateInstruction(instruction)).ToList();
    }

    public TreeNode[] ToOutputTree()
    {
        int nextNodeIndex = 1;
        var treeList = new List<TreeNode>();

        var queue = new Queue<BuilderTreeNode>();

        queue.Enqueue(this);

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
}