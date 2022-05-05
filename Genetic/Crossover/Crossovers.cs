namespace Genetic;
public static class Crossovers
{
    private static Random rnd = new Random();

    public static Crossover<Genotype> GetRandomCrossover(GenotypeStructure genotypeStructure) => 
        new Crossover<Genotype>()
    {
        Name = "Random crossover",
        Cross = (List<Genotype> parents) =>
        {
            var parentsAmount = parents.Count;
            var nodes = new Dictionary<int, int>();
            var parameters = new Dictionary<string, Dictionary<string, double>>();
            foreach (var orNodeIndex in genotypeStructure.TreeDimensions.Keys)
            {
                nodes.Add(orNodeIndex, parents[rnd.Next(parentsAmount)].Nodes[orNodeIndex]);
            }
            foreach (var sub in genotypeStructure.ParametersDimensions.Keys)
            {
                parameters.Add(sub, new Dictionary<string, double>());
                foreach (var param in genotypeStructure.ParametersDimensions[sub])
                {
                    parameters[sub].Add(param.Name, parents[rnd.Next(parentsAmount)].Parameters[sub][param.Name]);
                }
            }
            return new Genotype(nodes, parameters);
        }
    };
}
