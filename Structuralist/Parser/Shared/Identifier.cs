using System.Text.RegularExpressions;

namespace Structuralist.Parser;

public class Identifier : Token, Structuralist.MathExpression.IVariable
{
    public string Value { get; set; }
    public Identifier(string value, int stringNumber, int position)
        : base("identifier", stringNumber, position)
    {
        this.Value = value;
    }
}