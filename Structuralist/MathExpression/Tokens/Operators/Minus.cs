public class Minus : BinaryOperator
{
    public override int Calculate(int operand1, int operand2) => operand1 - operand2;
    public override Minus Copy() => new Minus();
    public override int GetPriority() => 2;
}