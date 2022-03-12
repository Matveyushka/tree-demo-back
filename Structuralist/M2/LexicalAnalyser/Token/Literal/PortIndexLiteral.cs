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

    public override string ToString()
    {
        return this.Index.ToString() + "_" + this.Direction;
    }
}

public class PortIndexLiteral : Token
{
    public PortIndex Value { get; set; }
    public PortIndexLiteral(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Terminal = new Terminal("portindex");
        this.Value = new PortIndex(int.Parse(value.Substring(0, value.Length - 1)), ExtractPortDirection(value));
    }

    public static bool Is(string value) =>
        int.TryParse(value.Substring(0, value.Length - 1), out var _) &&
        "wnesWNES".Contains(value[value.Length - 1]);

    private PortDirection ExtractPortDirection(string value) {
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

   public override string ToString() => Value.Index + Value.Direction switch {
            PortDirection.WEST => "w",
            PortDirection.NORTH => "n",
            PortDirection.EAST => "e",
            PortDirection.SOUTH => "s",
            _ => throw new Exception("AAAAAAAAAAA")
   };
}