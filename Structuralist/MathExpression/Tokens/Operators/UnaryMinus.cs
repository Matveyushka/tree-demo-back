public class UnaryMinus : UnaryOperator
{
    public override int Calculate(int operand) => -operand;
    public override UnaryMinus Copy() => new UnaryMinus();
    public override int GetPriority() => 1;
}