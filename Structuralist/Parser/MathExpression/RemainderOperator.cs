namespace Structuralist.MathExpression;
public class RemainderOperator : Operator
{
    public RemainderOperator(Operand op1, Operand op2) : base(op1, op2) { }

    public override int Count(Dictionary<string, int> variables) => 
        this.operand1.GetValue(variables) % this.operand2.GetValue(variables);
}