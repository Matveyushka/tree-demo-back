public class Limitation1D : ILimitation<Point1D>
{
    private Point1D limitPoint;
    private double direction;
    private double power;

    public Limitation1D(Point1D limitPoint, double direction, double power)
    {
        this.limitPoint = limitPoint;
        this.direction = direction > 0 ? 1 : -1;
        this.power = power;
    }

    public double GetPenalty(Point1D point)
    {
        var distance = point.X - limitPoint.X;
        var penalty = distance * direction > 0 
            ? distance * direction
            : 0;

        return Math.Max(0, Math.Pow(penalty + 1, power) - 1);
    }
}