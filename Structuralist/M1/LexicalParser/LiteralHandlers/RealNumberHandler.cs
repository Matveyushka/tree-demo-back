using System.Text.RegularExpressions;
using Structuralist.Parser;

namespace Structuralist.M1;

public class RealNumberHandler : LiteralHandler
{
    public override bool IsLexemeLiteral(string lexeme) => 
        new Regex(@"-?\d+\.\d+").IsMatch(lexeme);

    public override Token ParseLexeme(string lexeme, int stringNumber, int position) => 
        new RealNumber(double.Parse(lexeme.Replace('.', ',')), stringNumber, position);
}