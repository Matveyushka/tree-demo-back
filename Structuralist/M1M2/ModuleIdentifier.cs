namespace Structuralist.M1M2;

public class ModuleIdentifier
{
    public string Name { get; }
    public Dictionary<string, string> Features { get; } = new Dictionary<string, string>();

    public Dictionary<string, List<ModuleIdentifier>> Submodules { get; } = new Dictionary<string, List<ModuleIdentifier>>();

    public ModuleIdentifier(string name)
    {
        this.Name = name;
    }
}