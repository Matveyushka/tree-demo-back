namespace Structuralist.Parser;

public class Operator : Token
{
    public Operator(
        string value,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        this.Terminal = new Terminal(value);
    }
}