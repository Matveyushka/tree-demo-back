namespace Structuralist.Parser;

public class ShiftAction : ActionDecision
{
    public State NextState { get; set; }

    public ShiftAction(State nextState)
    {
        this.NextState = nextState;
    }

    public override bool Equals(object? obj) => obj is ShiftAction shift && 
        this.NextState.Equals(shift.NextState);

    public override int GetHashCode() => this.NextState.GetHashCode() + 31;

    public void Do(List<Token> input, Stack<State> stack, Stack<object> valueStack)
    {
        stack.Push(this.NextState);
        valueStack.Push(input[0]);
        input.RemoveAt(0);
    }
}