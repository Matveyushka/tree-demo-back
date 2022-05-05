namespace Structuralist.Parser;

public class NumberLiteral : Token, Structuralist.MathExpression.INumber
{
    public int Value { get; set; }
    public NumberLiteral(int value, int stringNumber, int position) 
        : base("number", stringNumber, position)
    {
        this.Value = value;
    }
}