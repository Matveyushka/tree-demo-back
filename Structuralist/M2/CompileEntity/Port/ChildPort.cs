namespace Structuralist.M2;

public class ChildPort : Port
{
    public string ModuleName { get; set; } = null!;
    public Structuralist.MathExpression.Expression ModuleIndex { get; set; } = null!;
}