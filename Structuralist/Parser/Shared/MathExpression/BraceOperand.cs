using Structuralist.MathExpression;

public class BraceOperand : Operand
{
    public Operator op;
    public BraceOperand(Operator op)
    {
        this.op = op;
    }

    public override int GetValue(Dictionary<string, int> variables) => op.Count(variables);
}