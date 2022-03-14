namespace Structuralist.MathExpression;
public abstract class Operator : Operand
{
    public Operand operand1;
    public Operand operand2;

    public Operator(Operand operand1, Operand operand2)
    {
        this.operand1 = operand1;
        this.operand2 = operand2;
    }

    public abstract int Count(Dictionary<string, int> variables);

    public override int GetValue(Dictionary<string, int> variables) => Count(variables);

    public override List<string> GetVariables() => operand1.GetVariables().Concat(operand2.GetVariables()).Distinct().ToList();
}