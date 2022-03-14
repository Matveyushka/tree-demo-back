namespace Structuralist.Parser;

public class FirstTable
{
    public Dictionary<NonTerminal, List<Terminal>> Table { get; set; }
        = new Dictionary<NonTerminal, List<Terminal>>();

    public FirstTable(Grammar grammar)
    {
        int changes = -1;
        grammar.Rules.ForEach(rule => {
            if (this.Table.ContainsKey(rule.Output) == false)
            {
                this.Table.Add(rule.Output, new List<Terminal>());
            }
        });

        while (changes != 0)
        {
            changes = 0;
            grammar.Rules.ForEach(rule =>
            {
                if (rule.Input[0] is Terminal terminal && this.Table[rule.Output].Contains(rule.Input[0]) == false)
                {
                    changes++;
                    this.Table[rule.Output].Add(terminal);
                }
                if (rule.Input[0] is NonTerminal nonTerminal)
                {
                    this.Table[nonTerminal].ForEach(terminal => {
                        if (this.Table[rule.Output].Contains(terminal) == false)
                        {
                            changes++;
                            this.Table[rule.Output].Add(terminal);
                        }
                    });
                }
            });
        }
    }

    public override string ToString() => this.Table.Aggregate("", (accumulator, value) => accumulator +
        value.Key + " : " + value.Value.Aggregate("", (a, v) => a + v + " ") + "\n");
}