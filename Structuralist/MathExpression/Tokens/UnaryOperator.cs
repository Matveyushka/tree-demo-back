namespace tempmath;
public abstract class UnaryOperator : Token
{
    abstract public int Calculate(int operand);
    abstract public int GetPriority();
}