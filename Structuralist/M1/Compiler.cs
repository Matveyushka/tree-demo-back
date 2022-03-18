namespace Structuralist.M1;

public static class M1Compiler
{
    public static TreeNode[] Compile(string code)
    {
        var tokens = new M1LexicalParser()
            .Parser
            .GetTokens(code);

        var model = new SyntaxParser()
            .GetModel(tokens);

        return new Builder()
            .Build(model);
    }
}