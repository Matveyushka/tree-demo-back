namespace Genetic;
public class InitializerGetter<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<Chromosome, Initializer<Chromosome>> Get { get; set; } = null!;
}
public class Initializer<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<int, List<Chromosome>> Initialize { get; set; } = null!;
}