namespace Structuralist.M2;

public class NumberLiteral : Token, Structuralist.MathExpression.INumber
{
    public int Value { get; set; }
    public NumberLiteral(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Terminal = new Terminal("number");
        this.Value = int.Parse(value);
    }

    public static bool Is(string value) => int.TryParse(value, out var _);
}