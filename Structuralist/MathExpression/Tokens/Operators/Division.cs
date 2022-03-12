namespace tempmath;

public class Division : BinaryOperator
{
    public override int Calculate(int operand1, int operand2) => operand1 / operand2;

    public override Division Copy() => new Division();

    public override int GetPriority() => 1;
}