using System;
using System.Collections.Generic;
using System.Text;

public class MathExpression
{
    enum State
    {
        BEGIN,
        AFTER_OPERATOR,
        AFTER_ORERAND,
        AFTER_OPEN_BRACE,
        AFTER_CLOSE_BRACE,
        ERROR,
    }

    List<Token> tokens = new List<Token>();
    List<Token> processingTokens = new List<Token>();
    List<string> allowedVariableNames = new List<string>();
    List<string> usedVariables = new List<string>();
    string operators = "()+-*/%";
    string expressionError = null;

    public string GetError() => expressionError;

    public bool IsVariableUsed(string name) => usedVariables.Contains(name);

    public int GetVariableAmount() => usedVariables.Count;

    public List<string> GetVariables() => usedVariables;

    public MathExpression(string source, List<string> allowedVariableNames)
    {
        this.allowedVariableNames = allowedVariableNames;
        StringBuilder operand = new StringBuilder("");
        char previousSymbol = '#';
        for (var symbolIndex = 0; symbolIndex != source.Length; symbolIndex++)
        {
            if (operators.Contains(source[symbolIndex]))
            {
                TryAddOperand(operand);
                AddOperator(source[symbolIndex], previousSymbol);
            }
            else if (!char.IsWhiteSpace(source[symbolIndex]))
            {
                operand.Append(source[symbolIndex]);
            }
            else
            {
                TryAddOperand(operand);
            }
            previousSymbol = source[symbolIndex];
        }
        TryAddOperand(operand);
        if (expressionError == null)
        {
            ValidateExpression();
        }
    }

    void CopyTokensToProcessing()
    {
        processingTokens.Clear();
        tokens.ForEach(token =>
        {
            processingTokens.Add(token.Copy());
        });
    }

    void ValidateExpression()
    {
        State state = State.BEGIN;
        int braceCounter = 0;
        tokens.ForEach(token =>
        {
            if (token is OpeningBrace)
            {
                braceCounter += 1;
            }
            else if (token is ClosingBrace)
            {
                braceCounter -= 1;
            }
            state = CheckNegativeBraceCounter(braceCounter, state);
            switch (state)
            {
                case State.BEGIN:
                    state = HandleBegin(token);
                    break;
                case State.AFTER_OPERATOR:
                    state = HandleAfterOperator(token);
                    break;
                case State.AFTER_ORERAND:
                    state = HandleAfterOperand(token);
                    break;
                case State.AFTER_OPEN_BRACE:
                    state = HandleAfterOpenBrace(token);
                    break;
                case State.AFTER_CLOSE_BRACE:
                    state = HandleAfterCloseBrace(token);
                    break;
                case State.ERROR:
                    return;
                default:
                    break;
            }
        });
        HandleEnd(state);
        if (braceCounter > 0)
        {
            expressionError = "Bad braces";
        }
    }

    State CheckNegativeBraceCounter(int braceCounter, State currentState)
    {
        if (braceCounter < 0)
        {
            expressionError = "Bad braces";
            return State.ERROR;
        }
        else
        {
            return currentState;
        }
    }

    State HandleBegin(Token token)
    {
        switch (token)
        {
            case BinaryOperator:
                expressionError = "Expression may not be started with binary operator";
                return State.ERROR;
            case UnaryOperator:
                return State.AFTER_OPERATOR;
            case OpeningBrace:
                return State.AFTER_OPEN_BRACE;
            case ClosingBrace:
                expressionError = "Expression may not be started with closing brace";
                return State.ERROR;
            case Operand:
                return State.AFTER_ORERAND;
            default:
                return State.ERROR;
        }
    }

    State HandleAfterOperand(Token token)
    {
        switch (token)
        {
            case BinaryOperator:
                return State.AFTER_OPERATOR;
            case UnaryOperator:
                expressionError = "Unary operator can't go after operand";
                return State.ERROR;
            case OpeningBrace:
                expressionError = "Opening brace can't go after operand";
                return State.ERROR;
            case ClosingBrace:
                return State.AFTER_CLOSE_BRACE;
            case Operand:
                expressionError = "Two operands cannot go in a row";
                return State.ERROR;
            default:
                return State.ERROR;
        }
    }

    State HandleAfterOperator(Token token)
    {
        switch (token)
        {
            case BinaryOperator:
                expressionError = "Two operators cannot go in a row";
                return State.ERROR;
            case UnaryOperator:
                expressionError = "Two operators cannot go in a row";
                return State.ERROR;
            case OpeningBrace:
                return State.AFTER_OPEN_BRACE;
            case ClosingBrace:
                expressionError = "Closing brace can't go after operator";
                return State.ERROR;
            case Operand:
                return State.AFTER_ORERAND;
            default:
                return State.ERROR;
        }
    }

    State HandleAfterOpenBrace(Token token)
    {
        switch (token)
        {
            case BinaryOperator:
                expressionError = "Binary operator can't go after opening brace";
                return State.ERROR;
            case UnaryOperator:
                return State.AFTER_OPERATOR;
            case OpeningBrace:
                return State.AFTER_OPEN_BRACE;
            case ClosingBrace:
                expressionError = "Closing brace can't go right after opening brace";
                return State.ERROR;
            case Operand:
                return State.AFTER_ORERAND;
            default:
                return State.ERROR;
        }
    }

    State HandleAfterCloseBrace(Token token)
    {
        switch (token)
        {
            case BinaryOperator:
                return State.AFTER_OPERATOR;
            case UnaryOperator:
                expressionError = "Unary operator can't go after closing brace";
                return State.ERROR;
            case OpeningBrace:
                expressionError = "Opening operator can't go after closing brace";
                return State.ERROR;
            case ClosingBrace:
                return State.AFTER_CLOSE_BRACE;
            case Operand:
                expressionError = "Operand can't go after closing brace";
                return State.ERROR;
            default:
                return State.ERROR;
        }
    }

    void HandleEnd(State state)
    {
        switch (state)
        {
            case State.AFTER_OPEN_BRACE:
                expressionError = "Opening brace can't be the end of expression";
                break;
            case State.AFTER_OPERATOR:
                expressionError = "Operator can't be the end of expression";
                break;
            default:
                break;
        }
    }

    void AddOperator(char sign, char previousSymbol)
    {
        switch (sign)
        {
            case '+':
                if (previousSymbol == '(' || previousSymbol == '#')
                {
                    tokens.Add(new UnaryPlus());
                    break;
                }
                tokens.Add(new Plus());
                break;
            case '-':
                if (previousSymbol == '(' || previousSymbol == '#')
                {
                    tokens.Add(new UnaryMinus());
                    break;
                }
                tokens.Add(new Minus());
                break;
            case '*':
                tokens.Add(new Multiplication());
                break;
            case '/':
                tokens.Add(new Division());
                break;
            case '%':
                tokens.Add(new Mod());
                break;
            case '(':
                tokens.Add(new OpeningBrace());
                break;
            case ')':
                tokens.Add(new ClosingBrace());
                break;
            default:
                break;
        }
    }

    void TryAddOperand(StringBuilder operand)
    {
        if (operand.Length != 0)
        {
            HandleAsNumber(operand);
            HandleAsVariable(operand);
            operand.Clear();
        }
    }

    void HandleAsNumber(StringBuilder operand)
    {
        if (IsTokenNumber(operand))
        {
            tokens.Add(new Number(int.Parse(operand.ToString())));
        }
    }

    void HandleAsVariable(StringBuilder operand)
    {
        if (IsTokenVariable(operand))
        {
            ValidateVariableName(operand);
            var variableName = operand.ToString();
            tokens.Add(new Variable(variableName));
            if (usedVariables.Find(v => v == variableName) is null)
            {
                usedVariables.Add(variableName);
            }
        }
    }

    void ValidateVariableName(StringBuilder variable)
    {
        var variableString = variable.ToString();
        if (allowedVariableNames.Find(name => name.Equals(variableString)) is null)
        {
            expressionError = $"Variable {variableString} in math expression is invalid";
        }
    }

    bool IsTokenNumber(StringBuilder token)
    {
        return token[0] >= '0' && token[0] <= '9';
    }

    bool IsTokenVariable(StringBuilder token)
    {
        return token[0] < '0' || token[0] > '9';
    }

    public (int result, string error) Calculate(Dictionary<string, int> variableValues)
    {
        CopyTokensToProcessing();
        if (expressionError is not null)
        {
            return (0, expressionError);
        }
        ReplaceVariablesWithValues(variableValues);
        CalculateNextSubexpression(0);
        return ((processingTokens[0] as Number).value, expressionError);
    }

    void ReplaceVariablesWithValues(Dictionary<string, int> variableValues)
    {
        for (var tokenIndex = 0; tokenIndex != processingTokens.Count; tokenIndex++)
        {
            if (processingTokens[tokenIndex] is Variable variable)
            {
                if (variableValues.ContainsKey(variable.name))
                {
                    processingTokens[tokenIndex] = new Number(variableValues[variable.name]);
                }
                else
                {
                    expressionError = $"The value of {variable.name} variable is not provided";
                }
            }
        }
    }

    int CalculateNextSubexpression(int beginTokenIndex)
    {
        int currentIndex = beginTokenIndex;
        if (expressionError is not null)
        {
            return -1;
        }
        while (currentIndex != processingTokens.Count && processingTokens[currentIndex] is not ClosingBrace)
        {
            int indexCorrect = 0;
            indexCorrect += HandleBraces(currentIndex);
            currentIndex += indexCorrect + 1;
            if (expressionError is not null)
            {
                return -1;
            }
        }
        currentIndex = beginTokenIndex;
        while (currentIndex != processingTokens.Count && processingTokens[currentIndex] is not ClosingBrace)
        {
            int indexCorrect = 0;
            indexCorrect += HandleUnaryOperator(currentIndex);
            indexCorrect += HandleBinaryOperator(currentIndex, 1);
            currentIndex += indexCorrect + 1;
            if (expressionError is not null)
            {
                return -1;
            }
        }
        currentIndex = beginTokenIndex;
        while (currentIndex != processingTokens.Count && processingTokens[currentIndex] is not ClosingBrace)
        {
            int indexCorrect = 0;
            indexCorrect += HandleUnaryOperator(currentIndex);
            indexCorrect += HandleBinaryOperator(currentIndex, 2);
            currentIndex += indexCorrect + 1;
            if (expressionError is not null)
            {
                return -1;
            }
        }
        return currentIndex;
    }

    int HandleBraces(int tokenIndex)
    {
        if (processingTokens[tokenIndex] is OpeningBrace)
        {
            int lastIndex = CalculateNextSubexpression(tokenIndex + 1);
            processingTokens.RemoveAt(tokenIndex);
            processingTokens.RemoveAt(lastIndex - 1);
            return -1;
        }
        return 0;
    }

    int HandleUnaryOperator(int tokenIndex)
    {
        if (processingTokens[tokenIndex] is UnaryOperator unaryOperator)
        {
            int result = unaryOperator.Calculate((processingTokens[tokenIndex + 1] as Number).value);
            processingTokens.Insert(tokenIndex, new Number(result));
            processingTokens.RemoveRange(tokenIndex + 1, 2);
            return -1;
        }
        return 0;
    }
    int HandleBinaryOperator(int tokenIndex, int priority)
    {
        if (processingTokens[tokenIndex] is BinaryOperator binaryOperator &&
            binaryOperator.GetPriority() == priority)
        {
            int result = binaryOperator.Calculate(
                (processingTokens[tokenIndex - 1] as Number).value,
                (processingTokens[tokenIndex + 1] as Number).value
            );
            processingTokens.RemoveRange(tokenIndex - 1, 3);
            processingTokens.Insert(tokenIndex - 1, new Number(result));
            return -2;
        }
        return 0;
    }
}