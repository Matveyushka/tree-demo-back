using System.Collections.Generic;

namespace Structuralist.M2;

public class SyntaxAnalyser
{
    private SyntaxAnalyserState state = new SyntaxAnalyserState();

    private void HandleTerminal(Terminal terminal, M2Model model, SyntaxAnalyserState state)
    {
        
    }

    public M2Model GetM2Model(List<Terminal> terminals)
    {
        M2Model model = new M2Model();
        for (var terminalIndex = 0; terminalIndex != terminals.Count; terminalIndex++)
        {
            var terminal = terminals[terminalIndex];
        }
        return model;
    }
}