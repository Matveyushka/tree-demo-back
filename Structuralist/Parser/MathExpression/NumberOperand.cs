using Structuralist.MathExpression;

public class NumberOperand : Operand
{
    public int value;
    public NumberOperand(int value)
    {
        this.value = value;
    }

    public override int GetValue(Dictionary<string, int> variables) => value;

    public override List<string> GetVariables() => new List<string>();
}