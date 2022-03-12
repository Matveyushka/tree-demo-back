namespace Structuralist.M2;

public class PositionRule
{
    public List<Func<int, int, bool>> Conditions { get; set; } 
        = new List<Func<int, int, bool>>();
    public Position Position { get; set; } = null!;
}