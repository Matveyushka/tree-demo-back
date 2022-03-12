namespace tempmath;
public abstract class BinaryOperator : Token
{  
    abstract public int Calculate(int operand1, int operand2);
    abstract public int GetPriority();
}