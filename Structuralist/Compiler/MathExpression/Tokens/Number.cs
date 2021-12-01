public class Number : Operand
{
    public int value;
    public Number(int value)
    {
        this.value = value;
    }

    public override Number Copy() => new Number(this.value);
}