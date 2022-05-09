using Genetic;
using Structuralist.M1M2;
using Structuralist.M1Tree;
using Structuralist.M2;

public class CircuitEstimator
{
    private readonly SpiceCktMapper cktMapper = new SpiceCktMapper();
    private readonly CircuitSimulator simulator = new CircuitSimulator();

    private double EstimateModule(Structuralist.M2.Output.Module module, List<Limitation2D> limitations)
    {
        var results = simulator.Simulate(cktMapper.Map(module));

        double estimation = 0;

        foreach (var res in results)
        {
            foreach (var limitation in limitations)
            {
                estimation += limitation.GetPenalty(new Point2D(res.Key, res.Value));
            }
        }

        return -estimation;
    }

    private readonly List<Limitation2D> lpfLimitations = new List<Limitation2D> {
        new Limitation2D(new Point2D(0, 0), new Point2D(6000, 0), 1, 1),
        new Limitation2D(new Point2D(0, -0.2), new Point2D(5000, -0.2), -1, 1),
        new Limitation2D(new Point2D(6000, -20), new Point2D(1e5, -20), 1, 1),
    };

    private readonly List<Limitation2D> hpfLimitations = new List<Limitation2D> {
        new Limitation2D(new Point2D(0, -20), new Point2D(3000, -20), 1, 1),
        new Limitation2D(new Point2D(3000, 0), new Point2D(1e5, 0), 1, 1),
        new Limitation2D(new Point2D(3500, -0.2), new Point2D(1e5, -0.2), -1, 1),
    };

    private readonly List<Limitation2D> bandLimitations = new List<Limitation2D> {
        new Limitation2D(new Point2D(0, -20), new Point2D(4000, -20), 1, 1),
        new Limitation2D(new Point2D(4000, 0), new Point2D(8000, 0), 1, 1),
        new Limitation2D(new Point2D(5000, -0.2), new Point2D(7000, -0.2), -1, 1),
        new Limitation2D(new Point2D(8000, -20), new Point2D(1e5, -20), 1, 1),
    };

    public Func<Genotype, double> GetEstimator(TreeNode[] tree, M2Model m2model) => id =>
        EstimateModule(m2model.GenerateStructure(ModuleIdentifier.ExtractFrom(tree.ToList(), id)!), lpfLimitations);
}