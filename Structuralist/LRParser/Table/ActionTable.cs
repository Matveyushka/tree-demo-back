public class ActionTable
{
    public Dictionary<State, Dictionary<Terminal, ActionDecision>> Table
        = new Dictionary<State, Dictionary<Terminal, ActionDecision>>();

    private void HandleShiftable(
        Position position,
        State state,
        GotoTable gotoTable)
    {
        var term = position.Locus as Terminal;
        if (this.Table[state].ContainsKey(term!) &&
            this.Table[state][term!].Equals(new ShiftAction(gotoTable.Table[state][term!])) == false)
        {
            throw new Exception("CONFLICT!!!");
        }
        else
        {
            if (this.Table[state].ContainsKey(term!) == false)
            {
                this.Table[state].Add(term!, new ShiftAction(gotoTable.Table[state][term!]));
            }
        }
    }

    private void HandleReducable(State state, Position position)
    {
        position.Lookahead.ForEach(term =>
        {
            if (this.Table[state].ContainsKey(term))
            {
                throw new Exception("CONFLICT!!!");
            }
            else
            {
                if (term.Value == "$" && position.Rule.Output.Equals(new NonTerminal("Start")))
                {
                    this.Table[state].Add(term, new AcceptAction());
                }
                else
                {
                    this.Table[state].Add(term, new ReduceAction(position));
                }
            }
        });
    }

    private void HandleState(State state, GotoTable gotoTable)
    {
        this.Table.Add(state, new Dictionary<Terminal, ActionDecision>());

        state.Positions.ForEach(position =>
        {
            if (position.IsShiftable)
            {
                HandleShiftable(position, state, gotoTable);
            }
            else if (position.IsReducable)
            {
                HandleReducable(state, position);
            }
        });
    }

    public ActionTable(GotoTable gotoTable) => gotoTable
        .Table
        .Keys
        .ToList()
        .ForEach(state => HandleState(state, gotoTable));
}