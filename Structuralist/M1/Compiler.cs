namespace Structuralist.M1;

public static class M1Compiler
{
    public static M1Model Compile(string code)
    {
        var tokens = new M1LexicalParser()
            .Parser
            .GetTokens(code);

        var model = new SyntaxParser()
            .GetModel(tokens);

        var semanticErrors = new SemanticParser()
            .SemanticCheck(model);

        if (semanticErrors.Count > 0)
        {
            throw new SemanticErrorsException(semanticErrors);
        }

        return model;
    }
}