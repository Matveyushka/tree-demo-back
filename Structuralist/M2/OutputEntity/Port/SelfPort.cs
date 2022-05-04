namespace Structuralist.M2.Output;

public class SelfPort : Port
{
    public SelfPort(PortIndex portIndex) : base(portIndex)
    {
    }

    public override bool Equals(Port? other)
    {
        if (other is SelfPort otherSelfPort)
        {
            return this.PortIndex.Equals(otherSelfPort.PortIndex);
        }

        return false;
    }
}