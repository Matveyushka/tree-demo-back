namespace Structuralist.M2.Output;

public class RectangleModule : Module
{
    public int West { get; }
    public int North { get; }
    public int East { get; }
    public int South { get; }

    public RectangleModule(
        int west, 
        int north, 
        int east, 
        int south, 
        string name,
        Dictionary<string, List<Module>> submodules, 
        List<Link> links,        
        Dictionary<string, string> features,
        int? x,
        int? y)
         : base(name, submodules, links, features, x, y)
    {
        this.West = west;
        this.North = north;
        this.East = east;
        this.South = south;
    }
}