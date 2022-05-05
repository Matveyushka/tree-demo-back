namespace Genetic;
public static class Mutagens
{
    private static Random rnd = new Random();

    private static Mutagen<Genotype> getUniformMutagenWithRate(
        GenotypeStructure genotypeStructure,
        string name,
        double rate)
    {
        return new Mutagen<Genotype>()
        {
            Name = name,
            Mutate = (Genotype chromosome) =>
            {
                var nodes = new Dictionary<int, int>();
                var parameters = new Dictionary<string, Dictionary<string, double>>();

                foreach (var orNodeIndex in genotypeStructure.TreeDimensions.Keys)
                {
                    var valueToChange = rnd.Next(genotypeStructure.TreeDimensions[orNodeIndex]);
                    nodes.Add(orNodeIndex, (rnd.NextDouble() > (1 - rate)) ?
                        rnd.Next(valueToChange) :
                        chromosome.Nodes[orNodeIndex]);
                }

                foreach(var sub in genotypeStructure.ParametersDimensions.Keys)
                {
                    parameters.Add(sub, new Dictionary<string, double>());
                    foreach (var param in genotypeStructure.ParametersDimensions[sub])
                    {
                        parameters[sub].Add(param.Name, param.LowerBound + rnd.NextDouble() * (param.UpperBound - param.LowerBound));
                    }
                }

                return new Genotype(nodes, parameters);
            }
        };
    }

    private static Mutagen<Genotype> getBoundaryMutagenWithRate(
        GenotypeStructure genotypeStructure,
        string name,
        double rate)
    {
        return new Mutagen<Genotype>()
        {
            Name = name,
            Mutate = (Genotype chromosome) =>
            {
                var nodes = new Dictionary<int, int>();
                var parameters = new Dictionary<string, Dictionary<string, double>>();

                foreach (var orNodeIndex in genotypeStructure.TreeDimensions.Keys)
                {
                    var valueToChange = rnd.Next(genotypeStructure.TreeDimensions[orNodeIndex]);
                    nodes.Add(orNodeIndex, (rnd.NextDouble() > (1 - rate)) ?
                        (
                            rnd.Next(2) > 1 ?
                            (chromosome.Nodes[orNodeIndex] + 1) % genotypeStructure.TreeDimensions[orNodeIndex] :
                            (chromosome.Nodes[orNodeIndex] - 1 + genotypeStructure.TreeDimensions[orNodeIndex]) % genotypeStructure.TreeDimensions[orNodeIndex]
                        ) :
                        chromosome.Nodes[orNodeIndex]);
                }

                foreach(var sub in genotypeStructure.ParametersDimensions.Keys)
                {
                    parameters.Add(sub, new Dictionary<string, double>());
                    foreach (var param in genotypeStructure.ParametersDimensions[sub])
                    {
                        var tenPercent = (param.UpperBound - param.LowerBound) / 10;

                        var value = chromosome.Parameters[sub][param.Name] - tenPercent / 2 + tenPercent * rnd.NextDouble();
                        value = Math.Max(value, param.LowerBound);
                        value = Math.Min(value, param.UpperBound);

                        parameters[sub].Add(param.Name, value);
                    }
                }

                return new Genotype(nodes, parameters);
            }
        };
    }

    public static Mutagen<Genotype> GetUselessMutagen(GenotypeStructure genotypeStructure) =>
        getUniformMutagenWithRate(genotypeStructure, "Useless mutagen", 0);
    public static Mutagen<Genotype> GetLightUniformMutagen(GenotypeStructure genotypeStructure) =>
        getUniformMutagenWithRate(genotypeStructure, "Light uniform mutagen (5%)", 0.05);
    public static Mutagen<Genotype> GetStrongUniformMutagen(GenotypeStructure genotypeStructure) =>
        getUniformMutagenWithRate(genotypeStructure, "Strong uniform mutagen (20%)", 0.2);
    public static Mutagen<Genotype> GetLightBoundaryMutagen(GenotypeStructure genotypeStructure) =>
        getBoundaryMutagenWithRate(genotypeStructure, "Light boundary mutagen (5%)", 0.05);
    public static Mutagen<Genotype> GetStrongBoundaryMutagen(GenotypeStructure genotypeStructure) =>
        getBoundaryMutagenWithRate(genotypeStructure, "Strong boundary mutagen (20%)", 0.2);

    public static MutagenGetter<GenotypeStructure, Genotype> UselessMutagenGetter 
        = new MutagenGetter<GenotypeStructure, Genotype>()
    {
        Name = "Useless mutagen",
        Get = GetUselessMutagen
    };
    public static MutagenGetter<GenotypeStructure, Genotype> LightUniformMutagenGetter 
        = new MutagenGetter<GenotypeStructure, Genotype>()
    {
        Name = "Light uniform mutagen (5%)",
        Get = GetLightUniformMutagen
    };
    public static MutagenGetter<GenotypeStructure, Genotype> StrongUniformMutagenGetter 
        = new MutagenGetter<GenotypeStructure, Genotype>()
    {
        Name = "Strong uniform mutagen (20%)",
        Get = GetStrongUniformMutagen
    };
    public static MutagenGetter<GenotypeStructure, Genotype> LightBoundaryMutagenGetter 
        = new MutagenGetter<GenotypeStructure, Genotype>()
    {
        Name = "Light boundary mutagen (5%)",
        Get = GetLightBoundaryMutagen
    };
    public static MutagenGetter<GenotypeStructure, Genotype> StrongBoundaryMutagenGetter 
        = new MutagenGetter<GenotypeStructure, Genotype>()
    {
        Name = "Strong boundary mutagen (20%)",
        Get = GetStrongBoundaryMutagen
    };
}