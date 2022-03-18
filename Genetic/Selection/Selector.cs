namespace Genetic;
public class Selector<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<List<EvaluatedIndividual<Chromosome>>, List<Chromosome>> Select { get; set; } = null!;
}