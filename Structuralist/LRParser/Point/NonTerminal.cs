public class NonTerminal : Point
{
    public NonTerminal(string value)
    {
        this.Value = value.ToUpper();
    }

    public override string ToString() => this.Value.ToUpper();

    public override bool Equals(object? obj) => obj is NonTerminal nonTerminal && nonTerminal.Value == this.Value;

    public override int GetHashCode() => this.Value.GetHashCode() + 1;
}