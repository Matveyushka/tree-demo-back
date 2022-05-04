using Genetic;
using Microsoft.AspNetCore.Mvc;
using Structuralist.M1;
using Structuralist.M1M2;
using Structuralist.M2;

namespace tree_demo_back.Controllers
{
    public class SynthesisInput
    {
        public string M1Code { get; set; } = null!;
        public string M2Code { get; set; } = null!;
        public Structuralist.M1M2.ModuleIdentifier Identifier { get; set; } = null!;
    }

    [ApiController]
    [Route("synthesis")]
    public class SynthesisController : ControllerBase
    {
        private ILogger<SynthesisController> logger;

        public SynthesisController(ILogger<SynthesisController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Post(SynthesisInput input)
        {
            var tree = M1Compiler.Compile(input.M1Code);

            var m2Model = M2Compiler.Compile(input.M2Code);

            var genotypeStructure = tree.GetGenotypeStructure();

            var geneticSolver = new GeneticSolver<Dictionary<int, int>>
            {
                PopulationSize = 50,
                Fitness = GetEstimator(tree, m2Model),
                Initializer = Initializers.getSimpleInitializer(genotypeStructure),
                Selector = Selectors.rankSelection,
                Crossover = Crossovers.randomCrossover,
                Mutagen = Mutagens.getLightUniformMutagen(genotypeStructure),
                Elite = 1
            };

            geneticSolver.Init();

            logger.LogInformation(geneticSolver.getBestEvaluation().ToString());

            var cktMapper = new SpiceCktMapper();

            var time = new TimeEstimator();
            var i = 0;
            while (true)
            {
                geneticSolver.nextGeneration();
                logger.LogInformation("Generation {1}: {2}", ++i, geneticSolver.getBestEvaluation());
            }

            logger.LogInformation("Result is: {1}", geneticSolver.getBestEvaluation());

            var ckt = cktMapper.Map(m2Model.GenerateStructure(ModuleIdentifier.ExtractFrom(tree.ToList(), geneticSolver.getOrderedPopulation().First().Chromo)!));

            return Ok();
        }

        private double EstimateModule(Structuralist.M2.Output.Module module)
        {
            var cktMapper = new SpiceCktMapper();

            var ckt = cktMapper.Map(module);

            var simulator = new CircuitSimulator();

            var results = simulator.Simulate(ckt);

            double estimation = 0;

            for (int i = 0; i != results.Count; i++)
            {
                var res = results[i];
                if (i <= 29)
                {
                    if (res > 0.1)
                    {
                        estimation += Math.Pow((1 + res - 0.1) * (1 + res - 0.1) * (1 + res - 0.1), 2);
                    }
                    else if (res < -0.1)
                    {
                        estimation += Math.Pow((-0.1 - res + 1) * (-0.1 - res + 1) * (-0.1 - res + 1), 2);
                    }
                }
                else if (i <= 39)
                {
                    if (res > -10)
                    {
                        estimation += (res + 10) * (res + 10);
                    }
                }
                else
                {
                    if (res > -20)
                    {
                        estimation += (res + 20) * (res + 20);
                    }
                }
            }

            return -estimation;
        }

        private Func<Dictionary<int, int>, double> GetEstimator(TreeNode[] tree, M2Model m2model) => id =>
            EstimateModule(m2model.GenerateStructure(ModuleIdentifier.ExtractFrom(tree.ToList(), id)!));
    }
}