public class UnaryPlus : UnaryOperator
{
    public override int Calculate(int operand) => operand;

    public override UnaryPlus Copy() => new UnaryPlus();
    public override int GetPriority() => 1;
}