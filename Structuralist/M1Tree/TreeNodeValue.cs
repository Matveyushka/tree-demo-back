using Structuralist.M1;

namespace Structuralist.M1Tree;

public class TreeNodeValue
{
    public List<ModuleInstanceName> ModuleList { get; set; } = new List<ModuleInstanceName>();
    public Feature? Value { get; set; } = null!;

    public TreeNodeValue() { }

    public TreeNodeValue(TreeNodeValue source)
    {
        this.Value = source.Value is not null ? new Feature(source.Value) : null;
        this.ModuleList = source.ModuleList.Select(module => new ModuleInstanceName(module)).ToList();
    }

    public bool SameModuleList(TreeNodeValue treeNodeValue)
    {
        if (this.ModuleList.Count == treeNodeValue.ModuleList.Count)
        {
            for (var index = 0; index != this.ModuleList.Count; index++)
            {
                if (this.ModuleList[index].Equals(treeNodeValue.ModuleList[index]) == false)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}