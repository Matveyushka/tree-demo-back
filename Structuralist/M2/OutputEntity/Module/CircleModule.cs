namespace Structuralist.M2.Output;

public class CircleModule : Module
{
    public int PortQuantity { get; }

    public CircleModule(
        int portQuantity,
        string name,
        Dictionary<string, List<Module>> submodules,
        List<Link> links,
        Dictionary<string, string> features,
        int? x,
        int? y)
         : base(name, submodules, links, features, x, y)
    {
        this.PortQuantity = portQuantity;
    }
}