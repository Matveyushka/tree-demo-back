namespace Structuralist.Parser;

public class Grammar
{
    public List<GrammarRule> Rules { get; set; } = new List<GrammarRule>();

    public Grammar()
    {

    }

    public Grammar(List<GrammarRule> rules)
    {
        this.Rules.AddRange(rules);
    }

    public Grammar Combine(List<GrammarRule> rules)
    {
        this.Rules.AddRange(rules);
        return this;
    }

    public Grammar Combine(Grammar grammar)
    {
        this.Rules.AddRange(grammar.Rules);
        return this;
    }

    public Grammar WithMath(string number, string identifier) =>
        this.Combine(Structuralist.MathExpression.MathGrammar.GetInstance("number", "identifier"));

    public override string ToString() => Rules.Aggregate("", (a, v) => a + v.ToString() + "\n").Trim();
}