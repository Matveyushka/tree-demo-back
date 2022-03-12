namespace Structuralist.M2.Output;

public class CircleModule : Module
{
    public int PortQuantity { get; }

    public CircleModule(
        int portQuantity,
        Dictionary<string, List<Module>> submodules,
        List<Link> links,
        int? x,
        int? y)
         : base(submodules, links, x, y)
    {
        this.PortQuantity = portQuantity;
    }
}