namespace Structuralist.Parser;

public class Position
{
    public GrammarRule Rule { get; set; }
    public int Item { get; set; }
    public List<Terminal> Lookahead { get; set; }

    public Point? Locus => this.Item == this.Rule.Input.Count ? null : this.Rule.Input[Item];
    public Point? Next => this.Item >= this.Rule.Input.Count - 1 ? null : this.Rule.Input[Item + 1];

    public bool IsShiftable => this.Item != this.Rule.Input.Count && this.Locus is Terminal;

    public bool IsReducable => this.Item == this.Rule.Input.Count;

    public Position(GrammarRule rule, int item, List<Terminal> lookahead)
    {
        this.Rule = rule;
        if (item > rule.Input.Count)
        {
            throw new ArgumentException($"Rule {rule} cannot has position more than {rule.Input.Count}, but got {item}");
        }
        this.Item = item;
        this.Lookahead = new List<Terminal>();
        lookahead.ForEach(term => this.Lookahead.Add(new Terminal(term.Value)));
    }

    public override string ToString()
    {
        string accumulator = Rule.Output.ToString() + " : ";

        for (int i = 0; i != Rule.Input.Count; i++)
        {
            if (i == this.Item) 
            {
                accumulator += ". ";
            }
            accumulator += Rule.Input[i] + " ";
        }
        if (this.Item == Rule.Input.Count)
        {
            accumulator += ". ";
        }

        accumulator += "{ ";

        for (int i = 0; i != Lookahead.Count; i++)
        {
            accumulator += Lookahead[i] + " ";
        }

        accumulator += "}";
        return accumulator;
    }

    public bool CoreEquals(Position position) => 
        this.Rule.Equals(position.Rule) &&
        this.Item == position.Item;

    public override bool Equals(object? obj)
    {
        if (obj is Position position)
        {
            var coreEqual = CoreEquals(position);
            var lookaheadEqual = this.Lookahead.Count == position.Lookahead.Count;
            if (lookaheadEqual)
            {
                for (var i = 0; i != this.Lookahead.Count; i++)
                {
                    if (this.Lookahead[i].Equals(position.Lookahead[i]) == false)
                    {
                        lookaheadEqual = false;
                    }
                }
            }
            return coreEqual && lookaheadEqual;
        }
        return false;
    }

    public override int GetHashCode()
    {
        var ruleHase = this.Rule.GetHashCode();
        var itemHash = this.Item.GetHashCode();
        var lookaheadHash = this.Lookahead[0].GetHashCode();
        for (int i = 1; i < this.Lookahead.Count; i++)
        {
            lookaheadHash = lookaheadHash ^ this.Lookahead[i].GetHashCode();
        }
        return ruleHase ^ itemHash ^ lookaheadHash;
    }
}