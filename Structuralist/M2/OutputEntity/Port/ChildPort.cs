namespace Structuralist.M2.Output;

public class ChildPort : Port
{
    public string SubmoduleName { get; }
    public int SubmoduleIndex { get; }

    public ChildPort(string submoduleName, int submoduleIndex, PortIndex portIndex) : base(portIndex)
    {
        this.SubmoduleName = submoduleName;
        this.SubmoduleIndex = submoduleIndex;
    }

    public override bool Equals(Port? other)
    {
        if (other is ChildPort otherChildPort)
        {
            return this.SubmoduleName == otherChildPort.SubmoduleName
                && this.SubmoduleIndex == otherChildPort.SubmoduleIndex
                && this.PortIndex.Equals(otherChildPort.PortIndex);
        }

        return false;
    }
}