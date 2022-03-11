using System;

namespace Structuralist.M2;

public class Literal : Terminal
{
    public string Value { get; set; }
    public LiteralType Type { get; set; }
    public const string ruleFirst = "First";
    public const string ruleLast = "Last";

    public Literal(
        string terminal,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        this.Value = terminal;

        if (IsNumber(terminal))
        {
            this.Type = LiteralType.NUMBER;
        }
        else if (IsPortIndex(terminal))
        {
            this.Type = LiteralType.PORT_INDEX;
        }
        else if (IsRule(terminal))
        {
            this.Type = LiteralType.RULE;
        }
        else
        {
            throw new ArgumentException($"Terminal {terminal} is not literal");
        }
    }

    private static bool IsNumber(string terminal) => int.TryParse(terminal, out var _);

    private static  bool IsPortIndex(string terminal) =>
        int.TryParse(terminal.Substring(0, terminal.Length - 1), out var _) &&
        "wnesWNES".Contains(terminal[terminal.Length - 1]);

    private static  bool IsRule(string terminal) =>
        terminal == ruleFirst ||
        terminal == ruleLast ||
        (
            int.TryParse(terminal.Substring(1, terminal.Length - 1), out var _) &&
            terminal[0] == '/'
        );

    public static bool IsLiteral(string terminal) =>
        Keyword.IsKeyword(terminal) == false &&
        (
            IsNumber(terminal) ||
            IsPortIndex(terminal) ||
            IsRule(terminal)
        );
}