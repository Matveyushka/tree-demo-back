using Structuralist.Parser;

namespace Structuralist.M2;

public class SyntaxParser
{
    SyntaxLR parser = new SyntaxLR(M2Grammar.Instance.WithMath("number", "identifier"));
    public M2Model GetModel(List<Token> input) => (M2Model)parser.Parse(input);
}