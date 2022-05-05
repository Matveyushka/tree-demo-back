namespace Genetic;
public static class Initializers
{
    public static Initializer<Genotype> GetSimpleInitializer(GenotypeStructure genotypeStructure)
    {
        return new Initializer<Genotype>()
        {
            Name = "Simple initializer",
            Initialize = (int size) => new int[size].Select(_ =>
            {
                Random rnd = new Random();

                var nodes = new Dictionary<int, int>();
                var parameters = new Dictionary<string, Dictionary<string, double>>();
                foreach (var key in genotypeStructure.TreeDimensions.Keys)
                {
                    nodes.Add(key, rnd.Next(genotypeStructure.TreeDimensions[key]));
                }
                foreach (var key in genotypeStructure.ParametersDimensions.Keys)
                {
                    parameters.Add(key, new Dictionary<string, double>());
                    foreach (var param in genotypeStructure.ParametersDimensions[key])
                    {
                        parameters[key].Add(param.Name, param.LowerBound + rnd.NextDouble() * (param.UpperBound - param.LowerBound));
                    }
                }
                return new Genotype(nodes, parameters);
            }).ToList()
        };
    }

    public static InitializerGetter<GenotypeStructure, Genotype> simpleInitializerGetter 
        = new InitializerGetter<GenotypeStructure, Genotype>()
    {
        Name = "Simple initializer",
        Get = GetSimpleInitializer
    };
}
