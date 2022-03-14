namespace Structuralist.Parser;

public class FollowTable
{
    public Dictionary<NonTerminal, List<Terminal>> Table { get; set; }
        = new Dictionary<NonTerminal, List<Terminal>>();

    private void InitTable(Grammar grammar)
    {
        grammar.Rules.ForEach(rule =>
        {
            if (this.Table.ContainsKey(rule.Output) == false)
            {
                this.Table.Add(rule.Output, new List<Terminal>());
            }
        });

        this.Table[new NonTerminal("Start")].Add(Terminal.InputEnd);
    }

    private int TerminalFollowRule(GrammarRule rule)
    {
        int changes = 0;
        for (var i = 0; i < rule.Input.Count - 1; i++)
        {
            if (rule.Input[i] is NonTerminal nonTerminal && rule.Input[i + 1] is Terminal terminal)
            {
                if (this.Table[nonTerminal].Contains(terminal) == false)
                {
                    changes++;
                    this.Table[nonTerminal].Add(terminal);
                }
            }
        }
        return changes;
    }

    private int OutputFollowRule(GrammarRule rule)
    {
        int changes = 0;
        if (rule.Input.Last() is NonTerminal nonTerminal)
        {
            this.Table[rule.Output].ForEach(terminal =>
            {
                if (this.Table[nonTerminal].Contains(terminal) == false)
                {
                    changes++;
                    this.Table[nonTerminal].Add(terminal);
                }
            });
        }
        return changes;
    }

    private int NonTerminalFollowRule(GrammarRule rule, FirstTable first)
    {
        int changes = 0;
        for (var i = 0; i < rule.Input.Count - 1; i++)
        {
            if (rule.Input[i] is NonTerminal nonTerminal && rule.Input[i + 1] is NonTerminal next)
            {
                first.Table[next].ForEach(terminal =>
                {
                    if (this.Table[nonTerminal].Contains(terminal) == false)
                    {
                        changes++;
                        this.Table[nonTerminal].Add(terminal);
                    }
                });
            }
        }
        return changes;
    }



    public FollowTable(Grammar grammar, FirstTable first)
    {
        int changes = -1;

        InitTable(grammar);

        while (changes != 0)
        {
            changes = 0;
            grammar.Rules.ForEach(rule =>
            {
                changes += TerminalFollowRule(rule);
                changes += NonTerminalFollowRule(rule, first);
                changes += OutputFollowRule(rule);
            });
        }
    }

    public override string ToString() => this.Table.Aggregate("", (accumulator, value) => accumulator +
        value.Key + " : " + value.Value.Aggregate("", (a, v) => a + v + " ") + "\n");
}