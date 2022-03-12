namespace Structuralist.MathExpression;
public class Expression
{
    private Operand operand { get; }
    public Expression(Operand operand)
    {
        this.operand = operand;
    }
    public int Calculate(Dictionary<string, int> variables) => this.operand.GetValue(variables);
}