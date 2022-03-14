using Structuralist.M2;

public class M2Compiler
{
    public M2Model? Compile(string input)
    {
        var tokens = new M2LexicalParser()
            .Parser
            .GetTokens(input);

        return new SyntaxParser()
            .GetModel(tokens);
    }
}