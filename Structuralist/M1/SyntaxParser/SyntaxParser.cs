using Structuralist.Parser;

namespace Structuralist.M1;

public class SyntaxParser
{
    SyntaxLR parser = new SyntaxLR(M1Grammar.Instance.WithMath("number", "identifier"));
    public M1Model GetModel(List<Token> input) => (M1Model)parser.Parse(input);
}