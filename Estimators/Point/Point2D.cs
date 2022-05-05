public class Point2D : Point
{
    public double X { get; private set; }
    public double Y { get; private set; }

    public Point2D(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }
}