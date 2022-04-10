namespace Genetic;
public static class Crossovers
{
    private static Random rnd = new Random();

    public static Crossover<Dictionary<int, int>> randomCrossover = new Crossover<Dictionary<int, int>>()
    {
        Name = "Random crossover",
        Cross = (List<Dictionary<int, int>> parents) =>
        {
            var parentsAmount = parents.Count;
            var orNodeIndices = parents[0].Keys;
            var chromosome = new Dictionary<int, int>();
            foreach (var orNodeIndex in orNodeIndices)
            {
                chromosome[orNodeIndex] = parents[rnd.Next(parentsAmount)][orNodeIndex];
                //chromosome[orNodeIndex] = parents[0][orNodeIndex];
            }
            return chromosome;
        }
    };

    public static Crossover<Dictionary<int, int>> partiallyMappedCrossover = new Crossover<Dictionary<int, int>>()
    {
        Name = "Partially mapped crossover",
        Cross = (List<Dictionary<int, int>> parents) =>
        {
            Random rnd = new Random((int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds % 10000 + Thread.CurrentThread.ManagedThreadId);

            var orNodeIndices = parents[0].Keys;
            var chromosome = new Dictionary<int, int>();

            var firstCut = rnd.Next(orNodeIndices.Count / 2);
            var secondCut = rnd.Next(orNodeIndices.Count / 2, orNodeIndices.Count);

            var counter = 0;
            foreach (var orNodeIndex in orNodeIndices)
            {
                if (counter < firstCut)
                {
                    chromosome[orNodeIndex] = parents[0][orNodeIndex];
                }
                else if (counter < secondCut)
                {
                    chromosome[orNodeIndex] = parents[1][orNodeIndex];
                }
                else
                {
                    chromosome[orNodeIndex] = parents[0][orNodeIndex];
                }
                counter++;
            }
            return chromosome;
        }
    };
}
