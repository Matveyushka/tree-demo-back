namespace Structuralist.M2;

public class Keyword : Token
{
    public static readonly string[] keywords = { 
        "Structura",
        "Position",
        "Place",
        "Feature",
        "Case",
        "List",
        "On", 
        "Link",
        "Port",
        "not"
    };
    
    public Keyword(
        string value,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        if (IsKeyword(value) == false)
        {
            throw new ArgumentException($"Terminal {value} is not keyword");
        }
        this.Terminal = new Terminal(value.ToLower());
    }

    public static bool IsKeyword(string terminal) => keywords.Contains(terminal);
}