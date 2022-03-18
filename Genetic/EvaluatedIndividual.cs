namespace Genetic;
public class EvaluatedIndividual<Chromosome>
{
    public Chromosome Chromo { get; }
    public double Evaluation { get; }

    public EvaluatedIndividual(
        Chromosome chromosome,
        double evaluation
    )
    {
        this.Chromo = chromosome;
        this.Evaluation = evaluation;
    }
}