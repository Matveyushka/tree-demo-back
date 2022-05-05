public interface ILimitation<P> where P : Point
{
    public double GetPenalty(P point);
}