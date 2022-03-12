namespace Structuralist.MathExpression;
public class MinusOperator : Operator
{
    public Operand operand1;
    public Operand operand2;

    public MinusOperator(Operand op1, Operand op2)
    {
        this.operand1 = op1;
        this.operand2 = op2;
    }

    public override int Count(Dictionary<string, int> variables) => 
        this.operand1.GetValue(variables) - this.operand2.GetValue(variables);
}