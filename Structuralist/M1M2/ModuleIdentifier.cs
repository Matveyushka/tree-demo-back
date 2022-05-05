using Genetic;
using Structuralist.M1Tree;

namespace Structuralist.M1M2;

public class ModuleIdentifier
{
    public string Name { get; set; } = null!;
    public int? Index { get; set; }
    public string? FullPath { get; set; } = null!;
    public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, List<ModuleIdentifier>> Submodules { get; set; } = new Dictionary<string, List<ModuleIdentifier>>();
    public Dictionary<string, double>? Parameters { get; set; } = new Dictionary<string, double>();

    public ModuleIdentifier()
    {
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

    private static void SetParameters(
        ModuleIdentifier id,
        Dictionary<string, Dictionary<string, double>> parameters)
    {
        if (id is not null)
        {
            id.Parameters = parameters[id.FullPath!];
            foreach (var name in id.Submodules)
            {
                foreach (var sub in id.Submodules[name.Key])
                {
                    SetParameters(sub, parameters);
                }
            }
        }
    }

    private static ModuleIdentifier? DivideContent(
        List<TreeNodeValue> content,
        Dictionary<string, Dictionary<string, double>> parameters)
    {
        if (content.Count == 0)
        {
            return null;
        }

        var id = new ModuleIdentifier
        {
            Name = content[0].ModuleList[0].Name,
            Index = 0,
            FullPath = $"{content[0].ModuleList[0].Name}[0]"
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
                        currentId.Submodules[module.Name].Add(new ModuleIdentifier
                        {
                            Name = module.Name,
                            Index = module.Index,
                            FullPath = $"{currentId.FullPath}{module.Name}[{module.Index}]"
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

        SetParameters(id, parameters);

        return id;
    }

    public static ModuleIdentifier? ExtractFrom(
        List<TreeNode> tree,
        Genotype genotype) =>
            DivideContent(ExtractContent(tree, genotype.Nodes), genotype.Parameters);
}