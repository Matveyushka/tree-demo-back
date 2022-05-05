namespace Structuralist.Parser;

public class Keyword : Token
{  
    public Keyword(
        string value,
        int stringNumber,
        int position
        ) : base(value, stringNumber, position)
    { }
}