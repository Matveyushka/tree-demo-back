using Structuralist.Parser;

namespace Structuralist.M2;

public class RuleLiteral : Token
{
    public string Value;

    public RuleLiteral(string value, int stringNumber, int position) 
        : base("rule", stringNumber, position)
    {
        this.Value = value;
    }
}