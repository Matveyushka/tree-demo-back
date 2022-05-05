namespace Structuralist.M1;

public class RealParameter
{
    public string Name { get; set; }
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }

    public RealParameter(string name, double lowerBound, double upperBound)
    {
        this.Name = name;
        this.LowerBound = lowerBound;
        this.UpperBound = upperBound;
    }

    public RealParameter(RealParameter other)
    {
        this.Name = other.Name;
        this.LowerBound = other.LowerBound;
        this.UpperBound = other.UpperBound;
    }
}