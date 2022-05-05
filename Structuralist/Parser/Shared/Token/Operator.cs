namespace Structuralist.Parser;

public class Operator : Token
{
    public Operator(
        string value,
        int stringNumber,
        int position
        ) : base(value, stringNumber, position)
    { }
}