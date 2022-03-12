namespace Structuralist.M2;

public class Position
{
    public Structuralist.MathExpression.Expression X { get; private set; }
    public Structuralist.MathExpression.Expression Y { get; private set; }

    public Position(
        Structuralist.MathExpression.Expression x, 
        Structuralist.MathExpression.Expression y)
        {
            this.X = x;
            this.Y = y;
        }
}