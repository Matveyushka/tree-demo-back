using Structuralist.M2;
using Structuralist.Parser;

public class NumberHandler : LiteralHandler
{
    public override bool IsLexemeLiteral(string lexeme) => int.TryParse(lexeme, out var _);

    public override Token ParseLexeme(string lexeme, int stringNumber, int position) =>
        new NumberLiteral(int.Parse(lexeme), stringNumber, position);
}