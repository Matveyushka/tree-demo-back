namespace Genetic;
public class InitializerGetter<ChromosomeStructure, Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<ChromosomeStructure, Initializer<Chromosome>> Get { get; set; } = null!;
}
public class Initializer<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<int, List<Chromosome>> Initialize { get; set; } = null!;
}