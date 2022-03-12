public class State
{
    public List<Position> Positions { get; set; } = new List<Position>();

    private List<Terminal> GetLookahead(Position position, FirstTable first, FollowTable follow)
    {
        if (position.Next is Terminal nextTerminal)
        {
            return new List<Terminal>() { nextTerminal };
        }
        else if (position.Next is NonTerminal nextNonTerminal)
        {
            return new List<Terminal>(first.Table[nextNonTerminal]);
        }
        else
        {
            return new List<Terminal>(follow.Table[position.Rule.Output]);
            //return new List<Terminal>(position.Lookahead);
        }
    }

    public State(Position initPosition, Grammar grammar, FirstTable first, FollowTable follow) : this(
        new List<Position>() { initPosition },
        grammar,
        first,
        follow
    )
    {

    }

    public State(List<Position> initPositions, Grammar grammar, FirstTable first, FollowTable follow)
    {
        Positions.AddRange(initPositions);
        int changes = -1;
        List<Position> newPositions = new List<Position>();
        while (changes != 0)
        {
            changes = 0;
            for (int i = 0; i != Positions.Count; i++)
            {
                var position = Positions[i];
                if (position.Locus is NonTerminal locus)
                {
                    var lookahead = GetLookahead(position, first, follow);
                    var locusPositions = grammar
                        .Rules
                        .Where(rule => rule.Output.Equals(locus))
                        .Select(rule => new Position(rule, 0, lookahead))
                        .ToList();
                    locusPositions.ForEach(locusPosition =>
                    {
                        if (Positions.Exists(p => p.CoreEquals(locusPosition)) == false)
                        {
                            Positions.Add(locusPosition);
                        }
                        else
                        {
                            var lp = Positions.First(p => p.CoreEquals(locusPosition));
                            locusPosition.Lookahead.ForEach(term =>
                            {
                                if (lp.Lookahead.Contains(term) == false)
                                {
                                    lp.Lookahead.Add(term);
                                }
                            });
                        }
                    });
                }
            }
        }
    }

    public override string ToString()
    {
        string accumulator = "{\n";
        this.Positions.ForEach(position => accumulator += position + "\n");
        accumulator += "}";
        return accumulator;
    }

    public bool CoreEquals(State state)
    {
        if (this.Positions.Count == state.Positions.Count)
        {
            for (int i = 0; i != this.Positions.Count; i++)
            {
                if (this.Positions[i].CoreEquals(state.Positions[i]) == false)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is State state)
        {
            if (this.Positions.Count == state.Positions.Count)
            {
                for (var i = 0; i != this.Positions.Count; i++)
                {
                    if (this.Positions[i].Equals(state.Positions[i]) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public override int GetHashCode()
    {
        var hash = this.Positions[0].GetHashCode();
        for (int i = 1; i < this.Positions.Count; i++)
        {
            hash = hash ^ this.Positions[i].GetHashCode();
        }
        return hash;
    }

    public static State InitState(Grammar grammar, FirstTable first, FollowTable follow) => new State(
        new Position(grammar.Rules[0], 0, new List<Terminal>() { new Terminal("$") }),
        grammar,
        first,
        follow
        );
}