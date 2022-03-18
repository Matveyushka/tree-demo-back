namespace Genetic;
public class Crossover<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<List<Chromosome>, Chromosome> Cross { get; set; } = null!;
}