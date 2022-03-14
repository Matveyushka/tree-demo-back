namespace Structuralist.Parser;

public class NumberLiteral : Token, Structuralist.MathExpression.INumber
{
    public int Value { get; set; }
    public NumberLiteral(int value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Terminal = new Terminal("number");
        this.Value = value;
    }
}