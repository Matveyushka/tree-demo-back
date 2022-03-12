using Structuralist.M2;

public class M2Compiler
{
    public M2Model? Compile(string input)
    {
        var lexicalAnalyser = new LexicalAnalyser();
        var syntaxAnalyser = new SyntaxAnalyser();

        List<Token> tokens = new List<Token>();

        try
        {
            tokens = lexicalAnalyser.GetTokens(input);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        try
        {
            return syntaxAnalyser.GetModel(tokens);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}