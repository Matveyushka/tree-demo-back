namespace Structuralist.MathExpression;

public static class Actions
{
    public static Func<List<object>, object> NumberOperand = values =>
        new NumberOperand(((INumber)values[0]).Value);

    public static Func<List<object>, object> VariableOperand = values =>
        new VariableOperand(((IVariable)values[0]).Value);

    public static Func<List<object>, object> BraceOperand = values => new BraceOperand((Operator)values[1]);

    public static Func<List<object>, object> Operand = values => (Operand)values[0];

    public static Func<List<object>, object> Remainder = values => new RemainderOperator((Operand)values[2], (Operand)values[0]);

    public static Func<List<object>, object> Multyply = values => new MultiplyOperator((Operand)values[2], (Operand)values[0]);

    public static Func<List<object>, object> Divide = values => new DivideOperator((Operand)values[2], (Operand)values[0]);

    public static Func<List<object>, object> Plus = values => new PlusOperator((Operand)values[2], (Operand)values[0]);

    public static Func<List<object>, object> Minus = values => new MinusOperator((Operand)values[2], (Operand)values[0]);

    public static Func<List<object>, object> Expression = values => new Expression((Operand)values[0]); 
}