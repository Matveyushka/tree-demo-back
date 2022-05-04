namespace Structuralist.M2.Output;

public abstract class Port : IEquatable<Port>
{
    public PortIndex PortIndex { get; }

    public Port(PortIndex portIndex)
    {
        this.PortIndex = portIndex;
    }

    public abstract bool Equals(Port? other);
}