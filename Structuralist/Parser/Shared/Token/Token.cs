namespace Structuralist.Parser;

public abstract class Token
{   
    public Terminal Terminal { get; set; } = null!;
    public int StringNumber { get; set; }
    public int Position { get; set; }
    public Token(string terminal, int stringNumber, int position)
    {
        this.Terminal = new Terminal(terminal);
        this.StringNumber = stringNumber;
        this.Position = position;
    }
}