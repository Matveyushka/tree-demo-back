using System.Text;

namespace Structuralist.Parser;

public class StringParser
{
    private int stringNumber;
    private List<Token> tokens = new List<Token>();
    private StringBuilder currentLexem = new StringBuilder();
    private LexicalKit lexicalKit;
    private string operatorSymbols;
    private bool processingOperator;

    public StringParser(LexicalKit lexicalKit)
    {
        this.lexicalKit = lexicalKit;

        operatorSymbols = string.Concat(
            lexicalKit
            .Operators
            .Aggregate((a, v) => a + v)
            .Distinct()
        );
    }

    private Token DetectToken(string lexeme, int stringNumber, int position)
    {
        if (lexicalKit.IsKeyword(lexeme))
        {
            return new Keyword(lexeme, stringNumber, position);
        }

        var literalHandler = lexicalKit
            .LiteralHandlers
            .FirstOrDefault(handler => handler.IsLexemeLiteral(lexeme));

        if (literalHandler is not default(LiteralHandler))
        {
            return literalHandler.ParseLexeme(lexeme, stringNumber, position);
        }

        if (lexicalKit.IsOperator(lexeme))
        {
            return new Operator(lexeme, stringNumber, position);
        }   
        if (lexicalKit.IsIdentifier(lexeme))
        {
            return new Identifier(lexeme, stringNumber, position);
        }



        throw new Exception($"Unknown lexem '{lexeme}'. String: {stringNumber}, position: {position}");
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
        if (operatorSymbols.Contains(symbol) && processingOperator == false)
        {
            AddToken(this.currentLexem.ToString(), position - 1);
            processingOperator = true;
            this.currentLexem.Clear();
            this.currentLexem.Append(symbol);
        }
        else if (operatorSymbols.Contains(symbol) == false && processingOperator && Char.IsWhiteSpace(symbol) == false)
        {
            AddToken(this.currentLexem.ToString(), position - 1);
            processingOperator = false;
            this.currentLexem.Clear();
            this.currentLexem.Append(symbol);
        }
        else if (Char.IsWhiteSpace(symbol))
        {
            AddToken(this.currentLexem.ToString(), position - 1);
            processingOperator = false;
            this.currentLexem.Clear();
        }
        else
        {
            if (processingOperator && lexicalKit.IsOperator(this.currentLexem.ToString() + symbol) == false)
            {
                AddToken(this.currentLexem.ToString(), position - 1);
                if (operatorSymbols.Contains(symbol) == false)
                {
                    processingOperator = false;
                }
                this.currentLexem.Clear();
            }
            this.currentLexem.Append(symbol);
        }
    }

    public List<Token> GetTokens(string inputString, int stringNumber)
    {
        this.stringNumber = stringNumber;
        this.tokens.Clear();
        this.currentLexem.Clear();
        this.processingOperator = false;

        for (var position = 0; position != inputString.Length; position++)
        {
            HandleInputSymbol(inputString[position], position);
        }
        AddToken(this.currentLexem.ToString(), inputString.Length - 1);

        return new List<Token>(this.tokens);
    }
}