using System.Text.RegularExpressions;

namespace Structuralist.Parser;

public class LexicalParser
{
    private LexicalKit lexicalKit;

    public LexicalParser(
        string[] operators,
        string[] keywords,
        Regex identifierRegex,
        List<LiteralHandler> literalHandlers
        )
    {
        this.lexicalKit = new LexicalKit(operators, keywords, identifierRegex, literalHandlers);
    }

    private List<string> DivideInputToStrings(string input) =>
        new List<string>(
            input
            .Split('\n')
        );

    public List<Token> GetTokens(string input)
    {
        var stringParser = new StringParser(lexicalKit);

        return DivideInputToStrings(input)
        .SelectMany((codeString, stringNumber) =>
            stringParser
            .GetTokens(codeString, stringNumber + 1)
        ).ToList();
    }
}