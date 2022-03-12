namespace Structuralist.M2;

public class RuleLiteral : Token
{
    public const string ruleFirst = "First";
    public const string ruleLast = "Last";
    public string Value;

    public RuleLiteral(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Terminal = new Terminal("rule");
        this.Value = value;
    }

    public static bool Is(string terminal) =>
        terminal == ruleFirst ||
        terminal == ruleLast ||
        (
            int.TryParse(terminal.Substring(1, terminal.Length - 1), out var _) &&
            terminal[0] == '\\'
        );
}