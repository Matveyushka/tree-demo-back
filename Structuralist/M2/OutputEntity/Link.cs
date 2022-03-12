namespace Structuralist.M2.Output;

public class Link
{
    public Port From { get; }
    public Port To { get; }
    
    public Link(Port from, Port to)
    {
        this.From = from;
        this.To = to;
    }
}