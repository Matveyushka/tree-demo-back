using Structuralist.Parser;

namespace Structuralist.M2;

public class RuleLiteral : Token
{
    public string Value;

    public RuleLiteral(string value, int stringNumber, int position) : base(stringNumber, position)
    {
        this.Terminal = new Terminal("rule");
        this.Value = value;
    }
}