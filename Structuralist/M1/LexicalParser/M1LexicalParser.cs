using System.Text.RegularExpressions;
using Structuralist.Parser;

namespace Structuralist.M1;

public class M1LexicalParser
{
    private string[] operators = new string[] {
        "(",
        ")",
        ",",
        "-",
        "+",
        "*",
        "/",
        "%"
    };

    private string[] keywords = new string[] {
        "Module",
        "Feature",
        "Case",
        "Constraint",
        "Restrict",
        "Generate",
        "modules",
        "Create",
        "Limit"
    };

    private Regex identifierRegex = new Regex("^[A-Z][a-zA-Z0-9]*$");

    private List<LiteralHandler> literalHandlers = new List<LiteralHandler>()
    {
        new NumberHandler(),
        new EnumHandler()
    };

    public LexicalParser Parser { get; }

    public M1LexicalParser()
    {
        this.Parser = new LexicalParser(
            operators, keywords, identifierRegex, literalHandlers
        );
    }
}