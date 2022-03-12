namespace Structuralist.M2.Output;

public class SimplePortIndex : PortIndex
{
    public int PortIndex { get; }

    public SimplePortIndex(int portIndex)
    {
        this.PortIndex = portIndex;
    }
}