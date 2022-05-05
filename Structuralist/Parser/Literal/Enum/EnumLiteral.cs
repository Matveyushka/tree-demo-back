namespace Structuralist.Parser;

public class EnumLiteral : Token
{
    public string Value { get; set; }
    public EnumLiteral(string value, int stringNumber, int position) 
        : base("enumvalue", stringNumber, position)
    {
        this.Value = value;
    }
}