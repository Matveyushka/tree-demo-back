namespace tempmath;
public class Plus : BinaryOperator
{
    public override int Calculate(int operand1, int operand2) => operand1 + operand2;
    public override Plus Copy() => new Plus();
    public override int GetPriority() => 2;
}