using Structuralist.MathExpression;

public class VariableOperand : Operand
{
    public string variableName;
    public VariableOperand(string variableName)
    {
        this.variableName = variableName;
    }

    public override int GetValue(Dictionary<string, int> variables) => variables[this.variableName];
}