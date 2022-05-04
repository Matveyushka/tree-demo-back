namespace Structuralist.M2.Output;

public class SimplePortIndex : PortIndex
{
    public int PortIndex { get; }

    public SimplePortIndex(int portIndex)
    {
        this.PortIndex = portIndex;
    }

    public override bool Equals(PortIndex? other)
    {
        if (other is SimplePortIndex otherPortIndex)
        {
            return this.PortIndex == otherPortIndex.PortIndex;
        }

        return false;
    }
}