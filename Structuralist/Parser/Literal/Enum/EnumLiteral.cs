namespace Structuralist.Parser;

public class EnumLiteral : Token
{
    public string Value { get; set; }
    public EnumLiteral(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Value = value;
        this.Terminal = new Terminal("enumvalue");
    }
}