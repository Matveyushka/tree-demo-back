using Structuralist.Parser;

namespace Structuralist.M1;

public class RealNumber : Token
{
    public double Value { get; private set; }
    public RealNumber(double value, int stringNumber, int position) 
        : base("real", stringNumber, position)
    {
        this.Value = value;
    }
}