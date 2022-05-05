public class Limitation2D : ILimitation<Point2D>
{
    private Point2D limitPoint1;
    private Point2D limitPoint2;
    private double direction;
    private double power;

    public Limitation2D(Point2D limitPoint1, Point2D limitPoint2, double direction, double power)
    {
        if (limitPoint2.X <= limitPoint1.X)
        {
            throw new ArgumentException("point2 X must be greater than point1 X");
        }
        this.limitPoint1 = limitPoint1;
        this.limitPoint2 = limitPoint2;
        this.direction = direction > 0 ? 1 : -1;
        this.power = power;
    }

    public double GetPenalty(Point2D point)
    {
        if (point.X > limitPoint2.X || point.X < limitPoint1.X)
        {
            return 0;
        }

        var proprotion = (point.X - limitPoint1.X) / (limitPoint2.X - limitPoint1.X);

        var maxY = Math.Max(limitPoint1.Y, limitPoint2.Y);
        var minY = Math.Min(limitPoint1.Y, limitPoint2.Y);

        var y = minY + (maxY - minY) * proprotion;

        var distance = point.Y - y;

        var penalty = distance * direction > 0 
            ? distance * direction
            : 0;

        return Math.Max(0, Math.Pow(penalty + 1, power) - 1);
    }
}