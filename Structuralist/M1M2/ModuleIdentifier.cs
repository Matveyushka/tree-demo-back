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
}