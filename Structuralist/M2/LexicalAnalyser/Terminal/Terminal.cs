namespace Structuralist.M2;

public abstract class Terminal
{   
    public int StringNumber { get; set; }
    public int Position { get; set; }
    public Terminal(int stringNumber, int position)
    {
        this.StringNumber = stringNumber;
        this.Position = position;
    }
}