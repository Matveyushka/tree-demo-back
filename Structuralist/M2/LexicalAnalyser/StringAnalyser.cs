using System;
using System.Collections.Generic;
using System.Text;

namespace Structuralist.M2;

public class StringAnalyser
{
    private int stringNumber;
    private List<Terminal> terminals = new List<Terminal>();
    private StringBuilder currentTerminal = new StringBuilder();

    private Terminal DetectTerminal(string terminal, int stringNumber, int position)
    {
        if (Keyword.IsKeyword(terminal))
        {
            return new Keyword(terminal, stringNumber, position);
        }
        else if (Literal.IsLiteral(terminal))
        {
            return new Literal(terminal, stringNumber, position);
        }
        else if (Operator.IsOperator(terminal))
        {
            return new Operator(terminal[0], stringNumber, position);
        }
        else if (Identifier.IsIdentifier(terminal))
        {
            return new Identifier(terminal, stringNumber, position);
        }
        else
        {
            throw new Exception($"Unknown terminal {terminal}. String: {stringNumber}, position: {position}");
        }
    }

    private void AddTerminal(string terminal, int lastSymbolPosition)
    {
        if (terminal.Length > 0)
        {
            this.terminals.Add(DetectTerminal(
                terminal, 
                this.stringNumber, 
                lastSymbolPosition - terminal.Length + 2));
        }
    }

    private void HandleInputSymbol(char symbol, int position)
    {
        if (Array.IndexOf(Operator.AllOperators, symbol) > -1)
        {
            AddTerminal(this.currentTerminal.ToString(), position - 1);
            AddTerminal(symbol.ToString(), position);
            this.currentTerminal.Clear();
        }
        else if (Char.IsWhiteSpace(symbol))
        {
            AddTerminal(this.currentTerminal.ToString(), position - 1);
            this.currentTerminal.Clear();
        }
        else
        {
            this.currentTerminal.Append(symbol);
        }
    }

    public List<Terminal> GetTerminals(string inputString, int stringNumber)
    {
        this.stringNumber = stringNumber;

        for (var position = 0; position != inputString.Length; position++)
        {
            HandleInputSymbol(inputString[position], position);
        }
        AddTerminal(this.currentTerminal.ToString(), inputString.Length - 1);

        return new List<Terminal>(this.terminals);
    }
}