using System.Text.RegularExpressions;

namespace Structuralist.Parser;

public class Identifier : Token, Structuralist.MathExpression.IVariable
{
    public string Value { get; set; }
    public Identifier(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Value = value;
        this.Terminal = new Terminal("identifier");
    }
}