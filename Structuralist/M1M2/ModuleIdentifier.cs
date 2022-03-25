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

    private static List<string> ExtractContent(TreeNode[] tree, Dictionary<int, int> genotype)
    {
        var content = new List<string>();
        var included = new List<int> { 0 };
        if (tree.Length == 0)
        {
            return content;
        }
        for (int index = 0; index != tree.Length; index++)
        {
            var node = tree[index];
            if (included.Contains(index))
            {
                content.Add(node.moduleList + "!");
                if (node.type == TreeNodeType.AND)
                {
                    node.children.ForEach(i => included.Add(i));
                }
                else if (node.children.Count > 0 && genotype.Count > 0)
                {
                    var gene = genotype[index];
                    included.Add(node.children[gene]);
                }
                else
                {
                    content.Add(node.content);
                }
            }
        }
        return content;
    }

    private static void HandleContent(List<string> content, ModuleIdentifier id)
    {
        if (content.Count == 3)
        {
            id.Name = content[0];
        }
        else if (content.Count == 4)
        {
            id.Features.Add(content[2], content[3]);
        }
        else if (content.Count % 2 == 1)
        {
            var submoduleName = content[content.Count - 3];
            var submoduleIndex = int.Parse(content[content.Count - 2]);
            if (id.Submodules.ContainsKey(submoduleName))
            {
                var subId = id.Submodules[submoduleName];
                if (subId.Count <= submoduleIndex)
                {
                    subId.Add(new ModuleIdentifier
                    {
                        Name = submoduleName,
                        Features = new Dictionary<string, string>(),
                        Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                    });
                }
            }
            else
            {
                id.Submodules.Add(submoduleName, new List<ModuleIdentifier> {
                    new ModuleIdentifier {
                        Name = submoduleName,
                        Features = new Dictionary<string, string>(),
                        Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                    }
                });
            }
        }
        else
        {
            if (id.Submodules.ContainsKey(content[2]))
            {
                var subId = id.Submodules[content[2]];
                var subIndex = int.Parse(content[3]);
                HandleContent(content.Skip(2).ToList(), subId[subIndex]);
            }
        }
    }

    private static ModuleIdentifier? DivideContent(List<string> content)
    {
        var divided = content
            .Select(str => str
                .Split(new char[] { '[', ']', ';', ':', '.', ' ' })
                .Where(token => token.Length > 0)
                .ToList()
            ).Where(s => s.Count > 0).ToList();

        if (divided.Count == 0)
        {
            return null;
        }

        var id = new ModuleIdentifier
        {
            Name = divided[0][0],
            Features = new Dictionary<string, string>(),
            Submodules = new Dictionary<string, List<ModuleIdentifier>>()
        };

        for (var i = 0; i != divided.Count; i++)
        {
            HandleContent(divided[i], id);
        }

        return id;
    }

    private static ModuleIdentifier? GetIdFromTree(TreeNode[]? tree, Dictionary<int, int> genotype) => tree is not null
        ? DivideContent(ExtractContent(tree, genotype))
        : null;

    public static ModuleIdentifier? ExtractFrom(TreeNode[] tree, Dictionary<int, int> genotype) => GetIdFromTree(tree, genotype);
}