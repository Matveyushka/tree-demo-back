namespace Genetic;
public static class Initializers
{
    private static Dictionary<int, int> generateChromosome(Dictionary<int, int> chromosomeDimensions)
    {
        Random rnd = new Random();

        var chromosome = new Dictionary<int, int>();
        foreach (var key in chromosomeDimensions.Keys)
        {
            chromosome[key] = rnd.Next(chromosomeDimensions[key]);
        }
        return chromosome;
    }

    public static Initializer<Dictionary<int, int>> getSimpleInitializer(Dictionary<int, int> chromosomeDimensions)
    {
        return new Initializer<Dictionary<int, int>>()
        {
            Name = "Simple initializer",
            Initialize = (int size) => new int[size].Select(_ => generateChromosome(chromosomeDimensions)).ToList()
        };
    }


    public static Initializer<Dictionary<int, int>> getDiversificationInitializer(Dictionary<int, int> chromosomeDimensions)
    {
        return new Initializer<Dictionary<int, int>>()
        {
            Name = "Simple initializer",
            Initialize = (int size) =>
            {
                Random rnd = new Random();

                var genotypeDensity = new Dictionary<int, int[]>();
                foreach (var gen in chromosomeDimensions)
                {
                    genotypeDensity[gen.Key] = new int[gen.Value];
                }

                var population = new List<Dictionary<int, int>>() {
                        generateChromosome(chromosomeDimensions)
                };

                foreach (var gen in chromosomeDimensions.Keys)
                {
                    genotypeDensity[gen][population[0][gen]]++;
                }

                while (population.Count < size)
                {
                    var newChromosome = new Dictionary<int, int>();

                    foreach (var gen in chromosomeDimensions.Keys)
                    {
                        var min = genotypeDensity[gen].Min();

                        var minIndexes = new List<int>();

                        for (int i = 0; i != genotypeDensity[gen].Count(); i++)
                        {
                            if (genotypeDensity[gen][i] == min)
                            {
                                minIndexes.Add(i);
                            }
                        }

                        var count = minIndexes.Count();

                        newChromosome[gen] = minIndexes[rnd.Next(count)];
                    }

                    population.Add(newChromosome);

                    foreach (var gen in chromosomeDimensions.Keys)
                    {
                        genotypeDensity[gen][population.Last()[gen]]++;
                    }
                }

                return population;
            }
        };
    }

    public static InitializerGetter<Dictionary<int, int>> simpleInitializerGetter = new InitializerGetter<Dictionary<int, int>>()
    {
        Name = "Simple initializer",
        Get = getSimpleInitializer
    };

    public static InitializerGetter<Dictionary<int, int>> diversificationInitializerGetter = new InitializerGetter<Dictionary<int, int>>()
    {
        Name = "Diversification initializer",
        Get = getDiversificationInitializer
    };
}
