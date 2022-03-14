using System.Text.RegularExpressions;

namespace Structuralist.Parser;

public class LexicalKit
{
    public string[] Operators { get; private set; }
    public string[] Keywords { get; private set; }
    public Regex IdentifierRegex { get; private set; }
    public List<LiteralHandler> LiteralHandlers { get; private set; }

    public LexicalKit(
        string[] operators,
        string[] keywords,
        Regex identifierRegex,
        List<LiteralHandler> literalHandlers
    )
    {
        this.Operators = operators;
        this.Keywords = keywords;
        this.IdentifierRegex = identifierRegex;
        this.LiteralHandlers = literalHandlers;
    }

    public bool IsOperator(string lexeme) => Array
        .FindIndex(
            this.Operators,
            op => op == lexeme
        ) >= 0;

    public bool IsKeyword(string lexeme) => Array
        .FindIndex(
            this.Keywords,
            op => op == lexeme
        ) >= 0;

    public bool IsIdentifier(string lexeme) => IdentifierRegex.IsMatch(lexeme);
}