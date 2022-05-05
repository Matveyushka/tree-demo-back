namespace Genetic;
public static class Selectors
{
    private static Genotype getRandomFromNormilized(List<EvaluatedIndividual<Genotype>> normilizedPopulation)
    {
        Random rnd = new Random();

        var random = rnd.NextDouble();

        var currentSum = 0.0;

        for (var i = 0; i < normilizedPopulation.Count; i++)
        {
            if (normilizedPopulation[i].Evaluation + currentSum > random)
            {
                return normilizedPopulation[i].Chromo;
            }
            currentSum += normilizedPopulation[i].Evaluation;
        }

        return normilizedPopulation[normilizedPopulation.Count - 1].Chromo;
    }

    private static List<Genotype> getRandomPairFromNormilized(List<EvaluatedIndividual<Genotype>> normilizedPopulation)
    {
        Random rnd = new Random();

        var random = rnd.NextDouble();

        var currentSum = 0.0;

        var first = normilizedPopulation[0];
        var second = normilizedPopulation[0];

        for (var i = 0; i < normilizedPopulation.Count; i++)
        {
            if (normilizedPopulation[i].Evaluation + currentSum > random)
            {
                first = normilizedPopulation[i];
                normilizedPopulation.RemoveAt(i);
                i = normilizedPopulation.Count;
            }
            else
            {
                currentSum += normilizedPopulation[i].Evaluation;
            }
        }

        currentSum = 0;
        random = rnd.NextDouble() * (1 - first.Evaluation);

        for (var i = 0; i < normilizedPopulation.Count; i++)
        {
            if (normilizedPopulation[i].Evaluation + currentSum > random)
            {
                second = normilizedPopulation[i];
                i = normilizedPopulation.Count;
            }
            else
            {
                currentSum += normilizedPopulation[i].Evaluation;
            }
        }

        return new List<Genotype>() {
                first.Chromo, second.Chromo
            };
    }

    public static Selector<Genotype> rouletteWheelSelection = new Selector<Genotype>()
    {
        Name = "Roulette wheel selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            double sum = evaluatedPopulation
                .Aggregate(0.0, (accumulator, value) => accumulator + value.Evaluation);

            var normilizedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Genotype>(
                individual.Chromo,
                individual.Evaluation / sum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    public static Selector<Genotype> rankSelection = new Selector<Genotype>()
    {
        Name = "Rank selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            var orderedPopualtion = evaluatedPopulation.OrderByDescending(individual => individual.Evaluation).ToList();

            var rankSum = orderedPopualtion.Count() * (1 + orderedPopualtion.Count()) / 2;

            var rank = orderedPopualtion.Count();

            var normilizedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Genotype>(
                individual.Chromo,
                (double)(rank--) / rankSum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    public static Selector<Genotype> tournamentSelection = new Selector<Genotype>()
    {
        Name = "Tournament selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            Random rnd = new Random();

            var firstIndividualIndex = rnd.Next(evaluatedPopulation.Count());
            var secondIndividualIndex = (firstIndividualIndex + rnd.Next(evaluatedPopulation.Count() - 2) + 1) % evaluatedPopulation.Count();

            var thirdIndividualIndex = rnd.Next(evaluatedPopulation.Count());
            var fourthIndividualIndex = (thirdIndividualIndex + rnd.Next(evaluatedPopulation.Count() - 2) + 1) % evaluatedPopulation.Count();

            var firstParent = evaluatedPopulation[firstIndividualIndex].Evaluation > evaluatedPopulation[secondIndividualIndex].Evaluation ?
                evaluatedPopulation[firstIndividualIndex].Chromo : evaluatedPopulation[secondIndividualIndex].Chromo;

            var secondParent = evaluatedPopulation[thirdIndividualIndex].Evaluation > evaluatedPopulation[fourthIndividualIndex].Evaluation ?
                evaluatedPopulation[thirdIndividualIndex].Chromo : evaluatedPopulation[fourthIndividualIndex].Chromo;

            return new List<Genotype>() {
                    firstParent, secondParent
            };
        }
    };

    public static Selector<Genotype> sigmaSelection = new Selector<Genotype>()
    {
        Name = "Sigma selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            var size = evaluatedPopulation.Count();

            var average = evaluatedPopulation
                .Aggregate(0.0, (acc, value) => acc + value.Evaluation) /
                size;

            var s = Math.Sqrt(
                (1.0 / (size - 1)) *
                (evaluatedPopulation
                .Aggregate(0.0, (acc, value) => acc + Math.Pow((value.Evaluation - average), 2)))
            );

            var sigmaEvaluatedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Genotype>(
                individual.Chromo,
                1 + (individual.Evaluation - average) / (2 * s)
            )).ToList();

            var sigmaSum = sigmaEvaluatedPopulation.Aggregate(0.0, (acc, value) => acc + value.Evaluation);

            var normilizedPopulation = sigmaEvaluatedPopulation.Select(individual => new EvaluatedIndividual<Genotype>(
                individual.Chromo,
                individual.Evaluation / sigmaSum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    private static int getGenotypeDifference(Genotype first, Genotype second)
    {
        int differenceCounter = 0;
        foreach (var entry in first.Nodes)
        {
            if (entry.Value != second.Nodes[entry.Key]) { differenceCounter++; }
        }
        return differenceCounter;
    }

    public static Selector<Genotype> genOutbreedingSelection = new Selector<Genotype>()
    {
        Name = "Genotype outbreeding selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            Random rnd = new Random();

            var firstParentIndex = rnd.Next(evaluatedPopulation.Count());

            var differences = evaluatedPopulation.Select(individual => getGenotypeDifference(evaluatedPopulation[firstParentIndex].Chromo, individual.Chromo));

            var maxDifference = differences.Max();

            var secondParentIndex = differences.ToList().FindIndex(difference => difference == maxDifference);

            return new List<Genotype>() {
                    evaluatedPopulation[firstParentIndex].Chromo, evaluatedPopulation[secondParentIndex].Chromo
            };
        }
    };

    public static Selector<Genotype> fenOutbreedingSelection = new Selector<Genotype>()
    {
        Name = "Phenotype outbreeding selection",
        Select = (List<EvaluatedIndividual<Genotype>> evaluatedPopulation) =>
        {
            Random rnd = new Random();

            var firstParentIndex = rnd.Next(evaluatedPopulation.Count());

            var differences = evaluatedPopulation.Select(individual => Math.Abs(evaluatedPopulation[firstParentIndex].Evaluation - individual.Evaluation));

            var maxDifference = differences.Max();

            var secondParentIndex = differences.ToList().FindIndex(difference => difference == maxDifference);

            return new List<Genotype>() {
                    evaluatedPopulation[firstParentIndex].Chromo, evaluatedPopulation[secondParentIndex].Chromo
            };
        }
    };
}