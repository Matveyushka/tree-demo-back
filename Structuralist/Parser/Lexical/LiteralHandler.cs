namespace Structuralist.Parser;

public abstract class LiteralHandler
{
    public abstract bool IsLexemeLiteral(string lexeme);

    public abstract Token ParseLexeme(string lexeme, int stringNumber, int position);
}