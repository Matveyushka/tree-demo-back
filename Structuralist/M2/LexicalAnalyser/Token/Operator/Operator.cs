using System;

namespace Structuralist.M2;

public class Operator : Token
{
    public const string operators = "()[],+-*/%";

    public Operator(
        char value,
        int stringNumber,
        int position
        ) : base(stringNumber, position)
    {
        if (operators.Contains(value) == false)
        {
            throw new ArgumentException($"Terminal {value} is not operator");
        }
        this.Terminal = new Terminal(value.ToString());
    }
    public static bool IsOperator(char value) => operators.Contains(value);

    public static bool IsOperator(string value) =>
        value.Length == 1 &&
        IsOperator(value[0]);
}