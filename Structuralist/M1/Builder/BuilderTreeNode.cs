namespace Structuralist.M1;

public struct ModuleInstanceName
{
    public string Name { get; set; }
    public int Index { get; set; }
}

public class BuilderTreeNode
{
    public TreeNodeType Type { get; set; }
    public List<BuilderTreeNode> Children { get; set; }
    public List<Feature> Content { get; set; }
    public Dictionary<string, int> SavedValues { get; set; }
    public List<ModuleInstanceName> ModuleList { get; set; }

    public List<ModuleGenerateInstruction> GenerateInstruction { get; set; }

    public BuilderTreeNode()
    {
        Children = new List<BuilderTreeNode>();
        Content = new List<Feature>();
        SavedValues = new Dictionary<string, int>();
        ModuleList = new List<ModuleInstanceName>();
        GenerateInstruction = new List<ModuleGenerateInstruction>();
    }

    public BuilderTreeNode(BuilderTreeNode source)
    {
        if (source is null)
        {
            throw new NullReferenceException("Copy constructor argument of BuilderTreeNode must not be null");
        }
        Type = source.Type;
        Content = source.Content.Select(c => new Feature(c)).ToList();
        Children = source.Children.Select(c => new BuilderTreeNode(c)).ToList();
        SavedValues = new Dictionary<string, int>(source.SavedValues);
        ModuleList = source.ModuleList.Select(c => c).ToList();
        GenerateInstruction = source.GenerateInstruction.Select(c => c).ToList();
    }

    public bool IsLeaf => this.Children.Count == 0;
}