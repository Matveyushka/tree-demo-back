namespace Structuralist.MathExpression;
public abstract class Operand
{
    public abstract int GetValue(Dictionary<string, int> variables);
    public abstract List<string> GetVariables();
}