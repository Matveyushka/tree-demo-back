using System;

namespace Structuralist.M2;

public class Operator : Terminal
{
    public OperatorType Type { get; set; }
    public const char braceOpen = '(';
    public const char braceClose = ')';
    public const char braceSquareOpen = '[';
    public const char braceSquareClose = ']';
    public const char comma = ',';
    public const char hyphen = '-';

    public Operator(
        char terminal,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        switch (terminal)
        {
            case braceOpen: this.Type = OperatorType.OPEN_BRACE; break;
            case braceClose: this.Type = OperatorType.CLOSE_BRACE; break;
            case braceSquareOpen: this.Type = OperatorType.OPEN_SQUARE_BRACE; break;
            case braceSquareClose: this.Type = OperatorType.CLOSE_SQUARE_BRACE; break;
            case comma: this.Type = OperatorType.COMMA; break;
            case hyphen: this.Type = OperatorType.HYPHEN; break;
            default: throw new ArgumentException($"Terminal {terminal} is not operator");
        }
    }
    public static bool IsOperator(char terminal) =>
        terminal == braceOpen ||
        terminal == braceClose ||
        terminal == braceSquareOpen ||
        terminal == braceSquareClose ||
        terminal == comma ||
        terminal == hyphen;

    public static bool IsOperator(string terminal) =>
        terminal.Length == 1 &&
        IsOperator(terminal[0]);

    public static readonly char[] AllOperators = {
        braceOpen,
        braceClose,
        braceSquareOpen,
        braceSquareClose,
        comma,
        hyphen
    };
}