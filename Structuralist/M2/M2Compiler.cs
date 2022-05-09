namespace Structuralist.M2;

public static class M2Compiler
{
    public static M2Model Compile(string input)
    {
        var tokens = new M2LexicalParser()
            .Parser
            .GetTokens(input);

        var model = new SyntaxParser()
            .GetModel(tokens);

        return model;
    }
}