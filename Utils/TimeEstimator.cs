class TimeEstimator
{
    DateTime timestamp;

    public void Begin()
    {
        this.timestamp = DateTime.Now;
    }

    public void End()
    {
        var endTime = DateTime.Now;
        Console.WriteLine((endTime - this.timestamp).TotalMilliseconds);
    }
}