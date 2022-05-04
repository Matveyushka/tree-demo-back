using Structuralist.M1;

namespace Structuralist.M1M2;

public class ModuleIdentifier
{
    public string Name { get; set; } = null!;
    public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, List<ModuleIdentifier>> Submodules { get; set; } = new Dictionary<string, List<ModuleIdentifier>>();

    public ModuleIdentifier()
    {
    }

    public ModuleIdentifier(string name)
    {
        this.Name = name;
    }

    private static List<TreeNodeValue> ExtractContent(List<TreeNode> tree, Dictionary<int, int> genotype)
    {
        var content = new List<TreeNodeValue>();
        var included = new List<int> { 0 };

        for (int index = 0; index != tree.Count; index++)
        {
            if (included[0] == index)
            {
                var node = tree[included[0]];
                if (node.Children.Count > 0)
                {
                    if (node.Type == TreeNodeType.AND)
                    {
                        included.AddRange(node.Children);
                    }
                    else if (genotype.Count > 0)
                    {
                        included.Add(node.Children[genotype[index]]);
                    }
                }
                else
                {
                    content.AddRange(node.Content);
                }
                included.RemoveAt(0);
                if (included.Count == 0)
                {
                    break;
                }
            }
        }

        return content;
    }

    private static ModuleIdentifier? DivideContent(List<TreeNodeValue> content)
    {
        if (content.Count == 0)
        {
            return null;
        }

        var id = new ModuleIdentifier
        {
            Name = content[0].ModuleList[0].Name,
            Features = new Dictionary<string, string>(),
            Submodules = new Dictionary<string, List<ModuleIdentifier>>()
        };

        content.ForEach(nodeValue =>
        {
            var currentId = id;
            for (int index = 0; index != nodeValue.ModuleList.Count; index++)
            {
                var module = nodeValue.ModuleList[index];
                if (index > 0)
                {
                    if (currentId.Submodules.ContainsKey(module.Name) == false)
                    {
                        currentId.Submodules.Add(module.Name, new List<ModuleIdentifier>());
                    }
                    var submodules = currentId.Submodules[module.Name];
                    while (currentId.Submodules[module.Name].Count <= module.Index - 1)
                    {
                        currentId.Submodules[module.Name].Add(null);
                    }
                    if (currentId.Submodules[module.Name].Count <= module.Index)
                    {
                        currentId.Submodules[module.Name].Add(new ModuleIdentifier{
                            Name = module.Name,
                            Features = new Dictionary<string, string>(),
                            Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                        });
                    }
                    currentId = currentId.Submodules[module.Name][module.Index];
                }
                if (index == nodeValue.ModuleList.Count - 1)
                {
                    if (nodeValue.Value is not null)
                    {
                        currentId.Features.Add(nodeValue.Value.Name, nodeValue.Value.Values[0]);
                    }
                }
            }
        });

        return id;
    }

    public static ModuleIdentifier? ExtractFrom(
        List<TreeNode> tree, 
        Dictionary<int, int> genotype) => 
        DivideContent(ExtractContent(tree, genotype));
}