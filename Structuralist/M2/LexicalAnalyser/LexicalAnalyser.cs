using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Structuralist.M2;

public class LexicalAnalyser
{
    private List<string> DivideInputToStrings(string input) =>
        new List<string>(
            input
            .Split('\n')
        );

    public List<Terminal> GetTerminals(string input) =>
        DivideInputToStrings(input)
        .SelectMany((codeString, stringNumber) => 
            new StringAnalyser().GetTerminals(codeString, stringNumber + 1)
        ).ToList();
}