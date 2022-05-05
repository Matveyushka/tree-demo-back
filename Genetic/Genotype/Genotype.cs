using Structuralist.M1;
using Structuralist.M1Tree;

namespace Genetic;

public class Genotype
{
    public Dictionary<int, int> Nodes { get; private set; }
    public Dictionary<string, Dictionary<string, double>> Parameters { get; private set; }

    public Genotype(Dictionary<int, int> nodes, Dictionary<string, Dictionary<string, double>> parameters)
    {
        this.Nodes = nodes;
        this.Parameters = parameters;
    }
}