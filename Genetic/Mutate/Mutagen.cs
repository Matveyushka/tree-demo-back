namespace Genetic;
public class MutagenGetter<ChromosomeStructure, Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<ChromosomeStructure, Mutagen<Chromosome>> Get { get; set; } = null!;
}
public class Mutagen<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<Chromosome, Chromosome> Mutate { get; set; } = null!;
}