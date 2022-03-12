public abstract class Token
{   
    public Terminal Terminal { get; set; } = null!;
    public int StringNumber { get; set; }
    public int Position { get; set; }
    public Token(int stringNumber, int position)
    {
        this.StringNumber = stringNumber;
        this.Position = position;
    }
}