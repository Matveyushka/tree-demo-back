namespace Structuralist.Parser;

public class SyntaxLR
{
    private Grammar grammar;
    private FirstTable firstTable;
    private FollowTable followTable;
    private GotoTable gotoTable;
    private ActionTable actionTable;

    public SyntaxLR(Grammar grammar)
    {
        this.grammar = grammar;
        this.firstTable = new FirstTable(this.grammar);
        this.followTable = new FollowTable(this.grammar, this.firstTable);
        this.gotoTable = new GotoTable(this.grammar, this.firstTable, this.followTable);
        this.actionTable = new ActionTable(this.gotoTable);
    }

    private void ThrowSyntaxError(List<Token> input, Stack<State> stack, Terminal read)
    {
        var expectedTerminals = this.actionTable
            .Table[stack.Peek()]
            .Keys
            .ToList()
            .Select(terminal => terminal.Value)
            .ToList();

        var expected = "";

        for (var terminalIndex = 0;
                terminalIndex != expectedTerminals.Count;
                terminalIndex++)
        {
            var term = expectedTerminals[terminalIndex];
            if (terminalIndex == 0)
            {
                expected += term;
            }
            else if (terminalIndex == expectedTerminals.Count - 1)
            {
                expected += $" or {term}";
            }
            else
            {
                expected += $", {term}";
            }
        }

        var errorMessage = $"Syntax error: Expected \"{expected}\" but got {read}.";

        if (read.Equals(Terminal.InputEnd) == false)
        {
            errorMessage += $" String: {input[0].StringNumber}. Position: {input[0].Position}";
        }

        throw new Exception(errorMessage);
    }

    private bool ParseStep(List<Token> input, Stack<State> stack, Stack<object> valueStack)
    {
        Terminal read;
        bool finished = false;
        if (input.Count == 0)
        {
            read = Terminal.InputEnd;
        }
        else
        {
            read = input[0].Terminal;
        }
        if (this.actionTable.Table[stack.Peek()].ContainsKey(read))
        {
            var currentAction = this.actionTable.Table[stack.Peek()][read];
            if (currentAction is ShiftAction shift)
            {
                shift.Do(input, stack, valueStack);
            }
            else if (currentAction is ReduceAction reduce)
            {
                reduce.Do(stack, valueStack, gotoTable);
            }
            else if (currentAction is AcceptAction)
            {
                finished = true;
            }
            else
            {
                throw new Exception("Unknown parse exception");
            }
        }
        else
        {
            ThrowSyntaxError(input, stack, read);
        }

        return finished;
    }

    public object Parse(List<Token> input)
    {
        Stack<State> stack = new Stack<State>();
        Stack<object> valueStack = new Stack<object>();

        stack.Push(State.InitState(this.grammar, this.firstTable, this.followTable));

        while (ParseStep(input, stack, valueStack) == false) { }

        return valueStack.Pop();
    }
}