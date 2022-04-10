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
                PopulationSize = 10,
                Fitness = BenchmarkTest.ModuleEstimator.GetEstimator(tree, m2Model),
                Initializer = Initializers.getSimpleInitializer(genotypeStructure),
                Selector = Selectors.rankSelection,
                Crossover = Crossovers.randomCrossover,
                Mutagen = Mutagens.getLightBoundaryMutagen(genotypeStructure),
                Elite = 1
            };

            geneticSolver.Init();

            //logger.LogInformation(geneticSolver.getBestEvaluation().ToString());

            var time = new TimeEstimator();
            time.Begin();
            for (int i = 0; i != 200; i++)
            {
                geneticSolver.nextGeneration();
                //logger.LogInformation("Generation {1}: {2}", i + 1, geneticSolver.getBestEvaluation());
            }
            time.End();


            logger.LogInformation("Result is: {1}", geneticSolver.getBestEvaluation());

            return Ok();
        }

        private double EstimateModule(Structuralist.M2.Output.Module module)
        {
            double estimation = 0;
            foreach (var submodules in module.Submodules)
            {
                foreach (var submodule in submodules.Value)
                {
                    estimation += EstimateModule(submodule);
                }
            }
            return estimation + 1 + module.Links.Count;
        }

        private Func<Dictionary<int, int>, double> GetEstimator(TreeNode[] tree, M2Model m2model) => id =>
            EstimateModule(m2model.GenerateStructure(ModuleIdentifier.ExtractFrom(tree.ToList(), id)!));
    }
}