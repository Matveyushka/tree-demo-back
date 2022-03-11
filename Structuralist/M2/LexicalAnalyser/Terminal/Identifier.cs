using System;
using System.Text.RegularExpressions;

namespace Structuralist.M2;

public class Identifier : Terminal
{
    public string Value { get; set; }
    public Identifier(string terminal, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Value = terminal;
        if (IsIdentifier(terminal) == false)
        {
            throw new ArgumentException($"Terminal {terminal} is not identitfier");
        }
    }

    private const string identifierPattern = "^[A-Z][a-zA-Z0-9]*$";
    private static readonly Regex identifierRegex = new Regex(identifierPattern);
    public static bool IsIdentifier(string terminal) => identifierRegex.IsMatch(terminal);
}