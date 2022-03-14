using System.Text.RegularExpressions;

namespace Structuralist.Parser;

public class EnumHandler : LiteralHandler
{
    private Regex enumValueRegex = new Regex("^[a-z][a-zA-Z0-9]*$");

    public override bool IsLexemeLiteral(string lexeme) => enumValueRegex.IsMatch(lexeme);

    public override Token ParseLexeme(string lexeme, int stringNumber, int position) =>
        new EnumLiteral(lexeme, stringNumber, position);
}