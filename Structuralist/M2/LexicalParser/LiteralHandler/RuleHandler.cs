using Structuralist.M2;
using Structuralist.Parser;

public class RuleHandler : LiteralHandler
{
    public const string ruleFirst = "First";
    public const string ruleLast = "Last";
    public override bool IsLexemeLiteral(string lexeme) => 
        lexeme == ruleFirst ||
        lexeme == ruleLast ||
        (
            int.TryParse(lexeme.Substring(1, lexeme.Length - 1), out var _) &&
            lexeme[0] == '\\'
        );

    public override Token ParseLexeme(string lexeme, int stringNumber, int position) =>
        new RuleLiteral(lexeme, stringNumber, position);
}