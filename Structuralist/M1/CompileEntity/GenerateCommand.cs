using Structuralist.MathExpression;

namespace Structuralist.M1;

public class GenerateCommand
{
    public Expression QuantityExpression { get; set; } = null!;
    public string ModuleName { get; set; } = null!;
    public string? Alias { get; set; } = null;
}