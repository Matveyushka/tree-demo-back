public class Terminal : Point
{
    public Terminal(string value)
    {
        this.Value = value.ToLower();
    }

    public override string ToString() => this.Value.ToLower();

    public override bool Equals(object? obj) => obj is Terminal terminal && terminal.Value == this.Value;

    public override int GetHashCode() => this.Value.GetHashCode() - 1;

    public static readonly Terminal InputEnd = new Terminal("$");
}