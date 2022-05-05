using Structuralist.M1;
using Structuralist.M1M2;
using Structuralist.M2;
using Genetic;
using BenchmarkDotNet.Attributes;
using Structuralist.M1Tree;

namespace BenchmarkTest;

[CsvExporter]
public class Test
{
   /* Random rnd = new Random();
    TreeNode[] tree = new M1TreeBuilder().Build(M1Compiler.Compile(CodeProvider.m1Code));
    List<TreeNode> treeList;
    M1Model m1Model = M1Compiler.Compile(CodeProvider.m1Code);
    M2Model m2Model = M2Compiler.Compile(CodeProvider.m2Code);
    //GeneticSolver<Dictionary<int, int>> GeneticSolver;

    Dictionary<int, int> GenotypeStructure;

    public IEnumerable<Dictionary<int, int>> RandomGenotypes { get; set; }
    public IEnumerable<ModuleIdentifier> RandomModulesIds { get; set; }
    public IEnumerable<Structuralist.M2.Output.Module> RandomStructures { get; set; }

    public Test()
    {
        //GenotypeStructure = GetGenotypeStructure();
        RandomGenotypes = new int[3].Select(_ => GetRandomGenotype());
        RandomModulesIds = RandomGenotypes.Select(gene => GetModuleId(gene));
        RandomStructures = RandomModulesIds.Select(id => GenerateStructure(id));
        treeList = tree.ToList();

        /*GeneticSolver = new GeneticSolver<Dictionary<int, int>>
        {
            PopulationSize = 10,
            Fitness = ModuleEstimator.GetEstimator(tree, m2Model),
            Initializer = Initializers.getSimpleInitializer(GenotypeStructure),
            Selector = Selectors.rankSelection,
            Crossover = Crossovers.randomCrossover,
            Mutagen = Mutagens.getLightBoundaryMutagen(GenotypeStructure),
            Elite = 1
        };

        GeneticSolver.Init();
    }

    //[Benchmark]
    public Dictionary<int, int> GetRandomGenotype()
    {
        var id = new Dictionary<int, int>();
        foreach (KeyValuePair<int, int> entry in GenotypeStructure)
        {
            id.Add(entry.Key, rnd.Next(entry.Value));
        }
        return id;
    }

    //[Benchmark]
    public GenotypeStructure GetGenotypeStructure()
    {
        return new GenotypeStructure(tree, m1Model);
    }

    //[Benchmark]
    [ArgumentsSource(nameof(RandomGenotypes))]
    public ModuleIdentifier GetModuleId(Dictionary<int, int> genotype)
    {
        return ModuleIdentifier.ExtractFrom(treeList, genotype)!;
    }

    //[Benchmark]
    [ArgumentsSource(nameof(RandomModulesIds))]
    public Structuralist.M2.Output.Module GenerateStructure(ModuleIdentifier moduleId)
    {
        return m2Model.GenerateStructure(moduleId);
    }

    //[Benchmark]
    [ArgumentsSource(nameof(RandomStructures))]
    public double EstimateModule(Structuralist.M2.Output.Module module)
    {
        return ModuleEstimator.EstimateModule(module);
    }

    //[Benchmark]
    public List<EvaluatedIndividual<Dictionary<int, int>>> EvaluatedPopulation()
    {
        return GeneticSolver.Population
            .Select(chromosome => new EvaluatedIndividual<Dictionary<int, int>>(
                chromosome, GeneticSolver.Fitness(chromosome)
            )).ToList();
    }

    //[Benchmark]
    public List<EvaluatedIndividual<Dictionary<int, int>>> OrderedPopulation()
    {
        return EvaluatedPopulation()
            .OrderByDescending(evaluatedIndividual => evaluatedIndividual.Evaluation)
            .ToList();
    }

    //[Benchmark]
    public List<EvaluatedIndividual<Dictionary<int, int>>> GetOrderedPopulation()
    {
        return GeneticSolver.getOrderedPopulation();
    }

    //[Benchmark]
    public List<List<Dictionary<int, int>>> Selection()
    {
        var orderedPopulation = GetOrderedPopulation();
        return new int[GeneticSolver.PopulationSize - GeneticSolver.Elite]
            .Select(_ => GeneticSolver.Selector.Select(orderedPopulation))
            .ToList();
    }

    //[Benchmark]
    public List<Dictionary<int, int>> GetChildren()
    {
        return Selection()
            .Select(selectedGroup => GeneticSolver.Crossover.Cross(selectedGroup))
            .ToList();
    }

    //[Benchmark]
    public List<Dictionary<int, int>> Mutate()
    {
        return GetChildren()
            .Select(child => GeneticSolver.Mutagen.Mutate(child))
            .ToList();
    }

    //[Benchmark]
    public List<Dictionary<int, int>> GetElite()
    {
        var orderedPopulation = GetOrderedPopulation();
        return orderedPopulation
            .Take(GeneticSolver.Elite)
            .Select(evaluatedIndividual => evaluatedIndividual.Chromo)
            .ToList();
    }

    //[Benchmark]
    public List<Dictionary<int, int>> NewPopulation()
    {
        return Mutate().Concat(GetElite()).ToList();
    }

    [Benchmark]
    public void Run()
    {
        GeneticSolver.nextGeneration();
    }*/
}