namespace Structuralist.Parser;

public class Keyword : Token
{  
    public Keyword(
        string value,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        this.Terminal = new Terminal(value);
    }
}