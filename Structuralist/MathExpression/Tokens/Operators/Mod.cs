namespace tempmath;

public class Mod : BinaryOperator
{
    public override int Calculate(int operand1, int operand2) => operand1 % operand2;
    public override Mod Copy() => new Mod();
    public override int GetPriority() => 1;
}