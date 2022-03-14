namespace Structuralist.M1;

public struct ModuleInstanceName
{
    public string name;
    public int index;
}

public class BuilderTreeNode
{
    public TreeNodeType Type { get; set; }
    public List<BuilderTreeNode> Children { get; set; }
    public List<Feature> Content { get; set; }
    public List<IntegerItemsGroupValue> SavedValues { get; set; }
    public List<ModuleInstanceName> ModuleList { get; set; }

    public List<ModuleGenerateInstruction> GenerateInstruction { get; set; }

    public BuilderTreeNode()
    {
        Children = new List<BuilderTreeNode>();
        Content = new List<Feature>();
        SavedValues = new List<IntegerItemsGroupValue>();
        ModuleList = new List<ModuleInstanceName>();
        GenerateInstruction = new List<ModuleGenerateInstruction>();
    }

    public BuilderTreeNode(BuilderTreeNode source)
    {
        Type = source.Type;
        Content = source.Content?.Select(c => new Feature() { Name = c.Name, Values = new List<string>(c.Values) }).ToList();
        Children = source.Children?.Select(c => new BuilderTreeNode(c)).ToList();
        SavedValues = source.SavedValues.Select(c => new IntegerItemsGroupValue(c)).ToList();
        ModuleList = source.ModuleList?.Select(c => c).ToList();
        GenerateInstruction = source.GenerateInstruction?.Select(c => c).ToList();
    }
}