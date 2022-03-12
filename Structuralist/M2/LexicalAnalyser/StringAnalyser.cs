using System.Text;

namespace Structuralist.M2;

public class StringAnalyser
{
    private int stringNumber;
    private List<Token> tokens = new List<Token>();
    private StringBuilder currentLexem = new StringBuilder();

    private Token DetectToken(string lexeme, int stringNumber, int position)
    {
        if (Keyword.IsKeyword(lexeme))
        {
            return new Keyword(lexeme, stringNumber, position);
        }
        else if (RuleLiteral.Is(lexeme))
        {
            return new RuleLiteral(lexeme, stringNumber, position);
        }
        else if (NumberLiteral.Is(lexeme))
        {
            return new NumberLiteral(lexeme, stringNumber, position);
        }
        else if (PortIndexLiteral.Is(lexeme))
        {
            return new PortIndexLiteral(lexeme, stringNumber, position);
        }
        else if (Operator.IsOperator(lexeme))
        {
            return new Operator(lexeme[0], stringNumber, position);
        }
        else if (Identifier.IsIdentifier(lexeme))
        {
            return new Identifier(lexeme, stringNumber, position);
        }
        else
        {
            throw new Exception($"Unknown lexem {lexeme}. String: {stringNumber}, position: {position}");
        }
    }

    private void AddToken(string lexem, int lastSymbolPosition)
    {
        if (lexem.Length > 0)
        {
            this.tokens.Add(DetectToken(
                lexem,
                this.stringNumber,
                lastSymbolPosition - lexem.Length + 2));
        }
    }

    private void HandleInputSymbol(char symbol, int position)
    {
        if (Operator.IsOperator(symbol))
        {
            AddToken(this.currentLexem.ToString(), position - 1);
            AddToken(symbol.ToString(), position);
            this.currentLexem.Clear();
        }
        else if (Char.IsWhiteSpace(symbol))
        {
            AddToken(this.currentLexem.ToString(), position - 1);
            this.currentLexem.Clear();
        }
        else
        {
            this.currentLexem.Append(symbol);
        }
    }

    public List<Token> GetTokens(string inputString, int stringNumber)
    {
        this.stringNumber = stringNumber;

        for (var position = 0; position != inputString.Length; position++)
        {
            HandleInputSymbol(inputString[position], position);
        }
        AddToken(this.currentLexem.ToString(), inputString.Length - 1);

        return new List<Token>(this.tokens);
    }
}