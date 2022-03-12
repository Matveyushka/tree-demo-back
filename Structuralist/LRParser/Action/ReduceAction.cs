public class ReduceAction : ActionDecision
{
    public Position ReducePosition { get; set; }

    public int Length => this.ReducePosition.Rule.Input.Count;
    public NonTerminal Result => this.ReducePosition.Rule.Output;

    public ReduceAction(Position reducePosition)
    {
        this.ReducePosition = reducePosition;
    }

    public void Do(Stack<State> stack, Stack<object> valueStack, GotoTable gotoTable)
    {
        var valueList = new List<object>();
        for (var _ = 0; _ != this.Length; _++)
        {
            stack.Pop();
            valueList.Add(valueStack.Pop());
        }
        var newValue = this.ReducePosition.Rule.Action.Invoke(valueList);
        valueStack.Push(newValue);
        stack.Push(gotoTable.Table[stack.Peek()][this.Result]);
    }
}