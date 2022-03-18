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
        if (content.Count == 4)
        {
            id.Name = content[0];
            id.Features.Add(content[2], content[3]);
        }
        else
        {
            if (id.Submodules.ContainsKey(content[2]))
            {
                var subId = id.Submodules[content[2]];
                var subIndex = int.Parse(content[3]);
                if (subId.Count <= subIndex)
                {
                    subId.Add(new ModuleIdentifier
                    {
                        Name = content[2],
                        Features = new Dictionary<string, string>(),
                        Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                    });
                }
                HandleContent(content.Skip(2).ToList(), subId[subIndex]);
            }
            else
            {
                id.Submodules.Add(content[2], new List<ModuleIdentifier>() { new ModuleIdentifier{
                        Name = content[2],
                        Features = new Dictionary<string, string>(),
                        Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                    } });

                if (id.Submodules.ContainsKey(content[2]))
                {
                    var subId = id.Submodules[content[2]];

                    var subIndex = int.Parse(content[3]);
                    if (subId.Count <= subIndex)
                    {
                        subId.Add(new ModuleIdentifier
                        {
                            Name = content[2],
                            Features = new Dictionary<string, string>(),
                            Submodules = new Dictionary<string, List<ModuleIdentifier>>()
                        });
                    }
                    HandleContent(content.Skip(2).ToList(), subId[subIndex]);
                }
            }
        }
    }

    private static ModuleIdentifier? DivideContent(List<string> content)
    {
        var divided = content.Select(str => str.Split(new char[] { '[', ']', ';', ':', '.', ' ' }).Where(s => s.Length > 0).ToList()).ToList();

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