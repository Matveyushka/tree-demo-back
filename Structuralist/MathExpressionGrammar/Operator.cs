namespace Structuralist.MathExpression;
public abstract class Operator : Operand
{
    public abstract int Count(Dictionary<string, int> variables);

    public override int GetValue(Dictionary<string, int> variables) => Count(variables);
}