namespace Structuralist.M2;

public class LinkRule
{
    public List<Func<int, int, bool>> Conditions { get; set; } 
        = new List<Func<int, int, bool>>();
    public List<LinkDescription> Links { get; set; } = new List<LinkDescription>();
}