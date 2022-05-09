using Genetic;
using Microsoft.AspNetCore.Mvc;
using Structuralist.M1;
using Structuralist.M1M2;
using Structuralist.M1Tree;
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
            var m1Model = M1Compiler.Compile(input.M1Code);

            var tree = new M1TreeBuilder().Build(m1Model);

            var m2Model = M2Compiler.Compile(input.M2Code);

            var genotypeStructure = new GenotypeStructure(tree, m1Model);

            var geneticSolver = new GeneticSolver<Genotype>
            {
                PopulationSize = 50,
                Fitness = new CircuitEstimator().GetEstimator(tree, m2Model),
                Initializer = Initializers.GetSimpleInitializer(genotypeStructure),
                Selector = Selectors.rankSelection,
                Crossover = Crossovers.GetRandomCrossover(genotypeStructure),
                Mutagen = Mutagens.GetStrongBoundaryMutagen(genotypeStructure),
                Elite = 1
            };

            geneticSolver.Init();

            logger.LogInformation(geneticSolver.getBestEvaluation().ToString());

            var cktMapper = new SpiceCktMapper();

            var time = new TimeEstimator();
            var i = 0;
            var e = -1000.0;
            var prevE = e;
            var platoCounter = 0;
            while (e < -1 && platoCounter < 200)
            {
                geneticSolver.nextGeneration();
                e = geneticSolver.getBestEvaluation();
                if (prevE == e)
                {
                    platoCounter++;
                }
                else
                {
                    prevE = e;
                    platoCounter = 0;
                }
                logger.LogInformation("Generation {1}: {2}", ++i, e);
            }

            logger.LogInformation("Result is: {1}", geneticSolver.getBestEvaluation());

            var bestChromo = geneticSolver.getOrderedPopulation().First().Chromo;

            var ckt = cktMapper.Map(m2Model.GenerateStructure(ModuleIdentifier.ExtractFrom(
                tree.ToList(),
                bestChromo)!));

            var writer = new CircuitFileWriter();

            writer.Write(ckt, @"C:\Users\user\Desktop\filter.ckt");

            geneticSolver.Fitness.Invoke(bestChromo);

            return Ok();
        }
    }
}