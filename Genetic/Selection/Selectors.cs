namespace Genetic;
public static class Selectors
{
    private static Dictionary<int, int> getRandomFromNormilized(List<EvaluatedIndividual<Dictionary<int, int>>> normilizedPopulation)
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

    private static List<Dictionary<int, int>> getRandomPairFromNormilized(List<EvaluatedIndividual<Dictionary<int, int>>> normilizedPopulation)
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

        return new List<Dictionary<int, int>>() {
                first.Chromo, second.Chromo
            };
    }

    public static Selector<Dictionary<int, int>> rouletteWheelSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Roulette wheel selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
        {
            double sum = evaluatedPopulation
                .Aggregate(0.0, (accumulator, value) => accumulator + value.Evaluation);

            var normilizedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Dictionary<int, int>>(
                individual.Chromo,
                individual.Evaluation / sum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    public static Selector<Dictionary<int, int>> rankSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Rank selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
        {
            var orderedPopualtion = evaluatedPopulation.OrderByDescending(individual => individual.Evaluation).ToList();

            var rankSum = orderedPopualtion.Count() * (1 + orderedPopualtion.Count()) / 2;

            var rank = orderedPopualtion.Count();

            var normilizedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Dictionary<int, int>>(
                individual.Chromo,
                (double)(rank--) / rankSum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    public static Selector<Dictionary<int, int>> tournamentSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Tournament selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
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

            return new List<Dictionary<int, int>>() {
                    firstParent, secondParent
            };
        }
    };

    public static Selector<Dictionary<int, int>> sigmaSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Sigma selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
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

            var sigmaEvaluatedPopulation = evaluatedPopulation.Select(individual => new EvaluatedIndividual<Dictionary<int, int>>(
                individual.Chromo,
                1 + (individual.Evaluation - average) / (2 * s)
            )).ToList();

            var sigmaSum = sigmaEvaluatedPopulation.Aggregate(0.0, (acc, value) => acc + value.Evaluation);

            var normilizedPopulation = sigmaEvaluatedPopulation.Select(individual => new EvaluatedIndividual<Dictionary<int, int>>(
                individual.Chromo,
                individual.Evaluation / sigmaSum
            )).ToList();

            return getRandomPairFromNormilized(normilizedPopulation);
        }
    };

    private static int getGenotypeDifference(Dictionary<int, int> first, Dictionary<int, int> second)
    {
        int differenceCounter = 0;
        foreach (var entry in first)
        {
            if (entry.Value != second[entry.Key]) { differenceCounter++; }
        }
        return differenceCounter;
    }

    public static Selector<Dictionary<int, int>> genOutbreedingSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Genotype outbreeding selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
        {
            Random rnd = new Random();

            var firstParentIndex = rnd.Next(evaluatedPopulation.Count());

            var differences = evaluatedPopulation.Select(individual => getGenotypeDifference(evaluatedPopulation[firstParentIndex].Chromo, individual.Chromo));

            var maxDifference = differences.Max();

            var secondParentIndex = differences.ToList().FindIndex(difference => difference == maxDifference);

            return new List<Dictionary<int, int>>() {
                    evaluatedPopulation[firstParentIndex].Chromo, evaluatedPopulation[secondParentIndex].Chromo
            };
        }
    };

    public static Selector<Dictionary<int, int>> fenOutbreedingSelection = new Selector<Dictionary<int, int>>()
    {
        Name = "Phenotype outbreeding selection",
        Select = (List<EvaluatedIndividual<Dictionary<int, int>>> evaluatedPopulation) =>
        {
            Random rnd = new Random();

            var firstParentIndex = rnd.Next(evaluatedPopulation.Count());

            var differences = evaluatedPopulation.Select(individual => Math.Abs(evaluatedPopulation[firstParentIndex].Evaluation - individual.Evaluation));

            var maxDifference = differences.Max();

            var secondParentIndex = differences.ToList().FindIndex(difference => difference == maxDifference);

            return new List<Dictionary<int, int>>() {
                    evaluatedPopulation[firstParentIndex].Chromo, evaluatedPopulation[secondParentIndex].Chromo
            };
        }
    };
}