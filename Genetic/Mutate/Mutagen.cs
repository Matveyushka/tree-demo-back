namespace Genetic;
public class MutagenGetter<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<Chromosome, Mutagen<Chromosome>> Get { get; set; } = null!;
}
public class Mutagen<Chromosome>
{
    public string Name { get; set; } = null!;
    public Func<Chromosome, Chromosome> Mutate { get; set; } = null!;
}