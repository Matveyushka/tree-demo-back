namespace Structuralist.M2.Output;

public abstract class Module
{
    public string Name { get; private set; }
    public int? X { get; private set; }
    public int? Y { get; private set; }
    public Dictionary<string, List<Module>> Submodules { get; private set; } 
        = new Dictionary<string, List<Module>>();
    public List<Link> Links { get; private set; } 
        = new List<Link>();

    public Module(
        string name,
        Dictionary<string, List<Module>> submodules, 
        List<Link> links,
        int? x,
        int? y)
    {
        this.Name = name;
        this.X = x;
        this.Y = y;
        this.Submodules = submodules;
        this.Links = links;
    }
}