namespace Structuralist.M2.Output;

public abstract class Port
{
    public PortIndex PortIndex { get; }

    public Port(PortIndex portIndex)
    {
        this.PortIndex = portIndex;
    }
}