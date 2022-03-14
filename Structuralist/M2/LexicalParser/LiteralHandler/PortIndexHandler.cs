using Structuralist.Parser;

namespace Structuralist.M2;

public class PortIndexHandler : LiteralHandler
{
    public override bool IsLexemeLiteral(string lexeme) => 
        int.TryParse(lexeme.Substring(0, lexeme.Length - 1), out var _) &&
        "wnesWNES".Contains(lexeme[lexeme.Length - 1]);

    public override Token ParseLexeme(string lexeme, int stringNumber, int position) =>
        new PortIndexLiteral(lexeme, stringNumber, position);
}