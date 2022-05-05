using Structuralist.Parser;

namespace Structuralist.M2;

public enum PortDirection
{
    WEST,
    NORTH,
    EAST,
    SOUTH
}

public class PortIndex
{
    public int Index { get; set; }
    public PortDirection Direction { get; set; }

    public PortIndex(int index, PortDirection direction)
    {
        this.Index = index;
        this.Direction = direction;
    }

    public override string ToString() => this.Index + this.Direction switch
    {
        PortDirection.WEST => "w",
        PortDirection.NORTH => "n",
        PortDirection.EAST => "e",
        PortDirection.SOUTH => "s",
        _ => throw new Exception("AAAAAAAAAAA")
    };
}

public class PortIndexLiteral : Token
{
    public PortIndex Value { get; set; }
    public PortIndexLiteral(string value, int stringNumber, int position) 
        : base("portindex", stringNumber, position)
    {
        this.Value = new PortIndex(int.Parse(value.Substring(0, value.Length - 1)), ExtractPortDirection(value));
    }

    private PortDirection ExtractPortDirection(string value)
    {
        var directionCode = value[value.Length - 1].ToString().ToLower();
        return directionCode switch
        {
            "w" => PortDirection.WEST,
            "n" => PortDirection.NORTH,
            "e" => PortDirection.EAST,
            "s" => PortDirection.SOUTH,
            _ => throw new ArgumentException($"Port direction code {directionCode} is not valid")
        };
    }

    public override string ToString() => this.Value.ToString();
}