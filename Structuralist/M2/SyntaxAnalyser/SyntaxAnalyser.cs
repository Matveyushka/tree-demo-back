namespace Structuralist.M2;

public class SyntaxAnalyser
{
    LRParser parser = new LRParser(
        M2Grammar.Instance.Combine(
            Structuralist.MathExpression.MathGrammar.GetInstance("number", "identifier")));
    public M2Model GetModel(List<Token> input) => (M2Model)parser.Parse(input);
}