namespace Structuralist.M2;

public class LexicalAnalyser
{
    private List<string> DivideInputToStrings(string input) =>
        new List<string>(
            input
            .Split('\n')
        );

    public List<Token> GetTokens(string input) =>
        DivideInputToStrings(input)
        .SelectMany((codeString, stringNumber) => 
            new StringAnalyser().GetTokens(codeString, stringNumber + 1)
        ).ToList();
}