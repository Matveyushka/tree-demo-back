namespace Structuralist.MathExpression;
public class Expression
{
    private Operand operand { get; }
    public Expression(Operand operand)
    {
        this.operand = operand;
    }
    public int Calculate(Dictionary<string, int> variables) => this.operand.GetValue(variables);

    public List<string> GetVariables() => this.operand.GetVariables().Distinct().ToList();

    public int GetVariablesQuantity() => this.GetVariables().Count;

    public bool IsVariableUsed(string variable) => this.GetVariables().Contains(variable);
}