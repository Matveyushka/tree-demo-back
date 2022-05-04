namespace Structuralist.M2.Output;

public class DirectedPortIndex : Structuralist.M2.Output.PortIndex
{
    public int PortIndex { get; }
    public PortDirection PortDirection { get; }

    public DirectedPortIndex(int portIndex, PortDirection portDirection)
    {
        this.PortIndex = portIndex;
        this.PortDirection = portDirection;
    }

    public override bool Equals(PortIndex? other)
    {
        if (other is DirectedPortIndex otherPortIndex)
        {
            return this.PortIndex == otherPortIndex.PortIndex
                && this.PortDirection == otherPortIndex.PortDirection;
        }

        return false;
    }
}