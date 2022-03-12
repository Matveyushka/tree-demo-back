using System;
using System.Text.RegularExpressions;

namespace Structuralist.M2;

public class Identifier : Token, Structuralist.MathExpression.IVariable
{
    public string Value { get; set; }
    public Identifier(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Value = value;
        this.Terminal = new Terminal("identifier");
        if (IsIdentifier(this.Value) == false)
        {
            throw new ArgumentException($"Terminal {this.Value} is not identitfier");
        }
    }

    private const string identifierPattern = "^[A-Z][a-zA-Z0-9]*$";
    private static readonly Regex identifierRegex = new Regex(identifierPattern);
    public static bool IsIdentifier(string terminal) => identifierRegex.IsMatch(terminal);
}