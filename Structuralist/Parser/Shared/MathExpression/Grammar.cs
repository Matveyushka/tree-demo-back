using Structuralist.Parser;

namespace Structuralist.MathExpression;

public static class MathGrammar
{
    public static Grammar GetInstance(
        string number,
        string variable) => new Grammar(new List<GrammarRule>()
        {
            GrammarRule.FromString("MATHEXPRESSION = PLUSMINUS", Actions.Expression),

            GrammarRule.FromString("PLUSMINUS = PLUSMINUS + MULTDIVIDE", Actions.Plus),
            GrammarRule.FromString("PLUSMINUS = PLUSMINUS - MULTDIVIDE", Actions.Minus),
            GrammarRule.FromString("PLUSMINUS = MULTDIVIDE", Actions.Operand),

            GrammarRule.FromString("MULTDIVIDE = MULTDIVIDE * OPERAND", Actions.Multyply),
            GrammarRule.FromString("MULTDIVIDE = MULTDIVIDE / OPERAND", Actions.Divide),
            GrammarRule.FromString("MULTDIVIDE = MULTDIVIDE % OPERAND", Actions.Remainder),
            GrammarRule.FromString("MULTDIVIDE = OPERAND", Actions.Operand),

            GrammarRule.FromString("OPERAND = ( PLUSMINUS )", Actions.BraceOperand),
            GrammarRule.FromString($"OPERAND = {number}", Actions.NumberOperand),
            GrammarRule.FromString($"OPERAND = {variable}", Actions.VariableOperand),
        });
}