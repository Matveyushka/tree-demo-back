namespace Structuralist.Parser;

public class GotoTable
{
    public Dictionary<State, Dictionary<Point, State>> Table { get; set; }
        = new Dictionary<State, Dictionary<Point, State>>();

    private void HandleState(List<State> states, Grammar grammar, FirstTable first, FollowTable follow)
    {
        List<Point> nextPoints = new List<Point>();
        var state = states[0];
        state.Positions.ForEach(position =>
        {
            if (position.Locus is not null)
            {
                if (nextPoints.Contains(position.Locus) == false)
                {
                    nextPoints.Add(position.Locus);
                }
            }
        });
        Dictionary<Point, State> partialTable = new Dictionary<Point, State>();
        nextPoints.ForEach(nextPoint =>
        {
            List<Position> nextPositions = new List<Position>();
            state.Positions.ForEach(position =>
            {
                if (position.Locus?.Equals(nextPoint) is true)
                {
                    nextPositions.Add(new Position(
                        position.Rule,
                        position.Item + 1,
                        position.Lookahead
                    ));
                }
            });
            var nextState = new State(nextPositions, grammar, first, follow);
            if (Table.ContainsKey(state) == false && states.Contains(nextState) == false)
            {
                states.Add(nextState);
            }
            partialTable.Add(nextPoint, nextState);
        });
        if (Table.ContainsKey(state) == false)
        {
            Table.Add(state, partialTable);
        }
        else 
        {
            partialTable.Keys.ToList().ForEach(key => {
                if (Table[state].ContainsKey(key) == false)
                {
                    Table[state].Add(key, partialTable[key]);
                }
            });
        }
        states.Remove(state);
    }

    public GotoTable(Grammar grammar, FirstTable first, FollowTable follow)
    {
        var initPosition = new Position(grammar.Rules[0], 0, new List<Terminal> {
            new Terminal("$")
        });
        var initState = new State(initPosition, grammar, first, follow);

        List<State> states = new List<State>() {
            initState
        };

        while (states.Count != 0)
        {
            HandleState(states, grammar, first, follow);
        }
    }
}