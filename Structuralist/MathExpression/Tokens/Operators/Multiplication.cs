namespace tempmath;
public class Multiplication : BinaryOperator
{
    public override int Calculate(int operand1, int operand2) => operand1 * operand2;
    public override Multiplication Copy() => new Multiplication();
    public override int GetPriority() => 1;
}