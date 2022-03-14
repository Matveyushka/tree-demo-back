using System.Text.RegularExpressions;
using Structuralist.Parser;

public class M2LexicalParser
{
    private string[] operators = new string[] {
        "(",
        ")",
        "[",
        "]",
        ",",
        "-",
        "+",
        "*",
        "/",
        "%"
    };

    private string[] keywords = new string[] {
        "Structura",
        "Position",
        "On",
        "Place",
        "Feature",
        "Case",
        "List",
        "Link",
        "Port",
        "not"
    };

    private Regex identifierRegex = new Regex("^[A-Z][a-zA-Z0-9]*$");

    private List<LiteralHandler> literalHandlers = new List<LiteralHandler>()
    {
        new NumberHandler(),
        new PortIndexHandler(),
        new RuleHandler()
    };

    public LexicalParser Parser { get; }

    public M2LexicalParser()
    {
        this.Parser = new LexicalParser(
            operators, keywords, identifierRegex, literalHandlers
        );
    }
}