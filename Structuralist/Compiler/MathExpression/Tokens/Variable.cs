public class Variable : Operand
{
    public string name;
    public Variable(string name)
    {
        this.name = name;
    }

    public override Variable Copy() => new Variable(this.name);
}