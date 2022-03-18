
namespace Genetic;

class GeneticSolver<Chromosome>
{
    public int PopulationSize { get; set; }
    public Func<Chromosome, double> Fitness { get; set; } = null!;
    public Initializer<Chromosome> Initializer { get; set; } = null!;
    public Selector<Chromosome> Selector { get; set; } = null!;
    public Crossover<Chromosome> Crossover { get; set; } = null!;
    public Mutagen<Chromosome> Mutagen { get; set; } = null!;
    public int Elite { get; set; }

    public List<Chromosome> Population { get; private set; } = new List<Chromosome>();

    public void Init()
    {
        Population = this.Initializer.Initialize(PopulationSize);
    }

    private List<EvaluatedIndividual<Chromosome>> getOrderedPopulation()
    {
        var evaluatedPopulation = this.Population
            .Select(chromosome => new EvaluatedIndividual<Chromosome>(
                chromosome, this.Fitness(chromosome)
            ));
        var sortedPopulation = evaluatedPopulation
            .OrderByDescending(evaluatedIndividual => evaluatedIndividual.Evaluation)
            .ToList();
        return sortedPopulation;
    }

    public void nextGeneration()
    {
        var sortedPopulation = getOrderedPopulation();
        var selection = new int[this.PopulationSize - this.Elite]
            .Select(_ => this.Selector.Select(sortedPopulation));
        var children = selection
            .Select(selectedGroup => this.Crossover.Cross(selectedGroup));
        var mutatedChildren = children
            .Select(child => this.Mutagen.Mutate(child));
        var elite = sortedPopulation
            .Take(this.Elite)
            .Select(evaluatedIndividual => evaluatedIndividual.Chromo);
        this.Population = mutatedChildren.Concat(elite).ToList();
    }

    public double getBestEvaluation()
    {
        return this.Fitness(getOrderedPopulation().First().Chromo);
    }

    public double getWorstEvaluation()
    {
        return this.Fitness(getOrderedPopulation().Last().Chromo);
    }

    public double getAverageEvaluation()
    {
        var evaluatedPopulation = this.Population
            .Select(chromosome => new EvaluatedIndividual<Chromosome>(
                chromosome, this.Fitness(chromosome)
            ));

        return evaluatedPopulation.Aggregate(0.0, (acc, value) => acc + value.Evaluation) / evaluatedPopulation.Count();
    }
}