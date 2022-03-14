namespace Structuralist.Parser;

public class GrammarRule
{
    public NonTerminal Output { get; set; } = null!;
    public List<Point> Input { get; set; } = new List<Point>();

    public Func<List<object>, object> Action { get; set; } = null!;

    private static bool IsUpper(char symbol) => symbol >= 'A' && symbol <= 'Z';
    private static bool IsTerminal(string point) => IsUpper(point[0]) == false;
    private static bool IsNonTerminal(string point) => IsUpper(point[0]);

    public static GrammarRule FromString(string stringRule, Func<List<object>, object> action)
    {
        GrammarRule rule = new GrammarRule();
        var parts = stringRule.Split(' ');
        if (IsNonTerminal(parts[0]) == false)
        {
            throw new ArgumentException($"Output in rule {stringRule} must be non terminal.");
        }
        rule.Output = new NonTerminal(parts[0]);
        for (var i = 2; i < parts.Length; i++)
        {
            if (IsTerminal(parts[i]))
            {
                rule.Input.Add(new Terminal(parts[i]));
            }
            else if (IsNonTerminal(parts[i]))
            {
                rule.Input.Add(new NonTerminal(parts[i]));
            }
        }
        rule.Action = action;
        return rule;
    }

    public override bool Equals(object? obj)
    {
        if (obj is GrammarRule rule && this.Output.Equals(rule.Output))
        {
            bool inputEquals = this.Input.Count == rule.Input.Count;
            if (inputEquals)
            {
                for (var i = 0; i != this.Input.Count; i++)
                {
                    if (this.Input[i] != rule.Input[i])
                    {
                        inputEquals = false;
                    }
                }
            }
            return inputEquals;
        }
        return false;
    }

    public override string ToString() => Output.ToString() + " : " + Input.Aggregate("", (a, v) => a + v.ToString() + " ").Trim();

    public override int GetHashCode()
    {
        var outHash = this.Output.GetHashCode();
        var inputHash = this.Input[0].GetHashCode();
        for (int i = 1; i < this.Input.Count; i++)
        {
            inputHash = inputHash ^ this.Input[i].GetHashCode();
        }
        return outHash ^ inputHash;
    }
}