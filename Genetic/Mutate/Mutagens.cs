namespace Genetic;
public static class Mutagens
{
    private static Random rnd = new Random();

    private static Mutagen<Dictionary<int, int>> getUniformMutagenWithRate(
        Dictionary<int, int> chromosomeDimensions,
        string name,
        double rate)
    {
        return new Mutagen<Dictionary<int, int>>()
        {
            Name = name,
            Mutate = (Dictionary<int, int> chromosome) =>
            {
                var orNodeIndices = chromosome.Keys;
                var mutatedChromosome = new Dictionary<int, int>();

                foreach (var orNodeIndex in orNodeIndices)
                {
                    var valueToChange = rnd.Next(chromosomeDimensions[orNodeIndex]);
                    mutatedChromosome[orNodeIndex] = (rnd.NextDouble() > (1 - rate)) ?
                        rnd.Next(chromosomeDimensions[orNodeIndex]) :
                        chromosome[orNodeIndex];
                }

                return mutatedChromosome;
            }
        };
    }

    private static Mutagen<Dictionary<int, int>> getBoundaryMutagenWithRate(
        Dictionary<int, int> chromosomeDimensions,
        string name,
        double rate)
    {
        return new Mutagen<Dictionary<int, int>>()
        {
            Name = name,
            Mutate = (Dictionary<int, int> chromosome) =>
            {
                var orNodeIndices = chromosome.Keys;
                var mutatedChromosome = new Dictionary<int, int>();

                foreach (var orNodeIndex in orNodeIndices)
                {
                    mutatedChromosome[orNodeIndex] = (rnd.NextDouble() > (1 - rate)) ?
                        ((rnd.NextDouble() > 0.5) ?
                            ((chromosome[orNodeIndex] + 1) % chromosomeDimensions[orNodeIndex]) :
                            ((chromosome[orNodeIndex] - 1 + chromosomeDimensions[orNodeIndex]) % chromosomeDimensions[orNodeIndex])) :
                        chromosome[orNodeIndex];
                }

                return mutatedChromosome;
            }
        };
    }

    public static Mutagen<Dictionary<int, int>> getUselessMutagen(Dictionary<int, int> chromosomeDimensions) =>
        getUniformMutagenWithRate(chromosomeDimensions, "Useless mutagen", 0);
    public static Mutagen<Dictionary<int, int>> getLightUniformMutagen(Dictionary<int, int> chromosomeDimensions) =>
        getUniformMutagenWithRate(chromosomeDimensions, "Light uniform mutagen (5%)", 0.05);
    public static Mutagen<Dictionary<int, int>> getStrongUniformMutagen(Dictionary<int, int> chromosomeDimensions) =>
        getUniformMutagenWithRate(chromosomeDimensions, "Strong uniform mutagen (20%)", 0.2);

    public static Mutagen<Dictionary<int, int>> getLightBoundaryMutagen(Dictionary<int, int> chromosomeDimensions) =>
        getBoundaryMutagenWithRate(chromosomeDimensions, "Light boundary mutagen (5%)", 0.05);
    public static Mutagen<Dictionary<int, int>> getStrongBoundaryMutagen(Dictionary<int, int> chromosomeDimensions) =>
        getBoundaryMutagenWithRate(chromosomeDimensions, "Strong boundary mutagen (20%)", 0.2);


    public static MutagenGetter<Dictionary<int, int>> UselessMutagenGetter = new MutagenGetter<Dictionary<int, int>>()
    {
        Name = "Useless mutagen",
        Get = getUselessMutagen
    };
    public static MutagenGetter<Dictionary<int, int>> LightUniformMutagenGetter = new MutagenGetter<Dictionary<int, int>>()
    {
        Name = "Light uniform mutagen (5%)",
        Get = getLightUniformMutagen
    };
    public static MutagenGetter<Dictionary<int, int>> StrongUniformMutagenGetter = new MutagenGetter<Dictionary<int, int>>()
    {
        Name = "Strong uniform mutagen (20%)",
        Get = getStrongUniformMutagen
    };
    public static MutagenGetter<Dictionary<int, int>> LightBoundaryMutagenGetter = new MutagenGetter<Dictionary<int, int>>()
    {
        Name = "Light boundary mutagen (5%)",
        Get = getLightBoundaryMutagen
    };
    public static MutagenGetter<Dictionary<int, int>> StrongBoundaryMutagenGetter = new MutagenGetter<Dictionary<int, int>>()
    {
        Name = "Strong boundary mutagen (20%)",
        Get = getStrongBoundaryMutagen
    };
}