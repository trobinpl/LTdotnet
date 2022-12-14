namespace FountainCodes;

/// <summary>
/// Provides values according to the Robust Solition Distribution
/// </summary>
public class RobustSolitionDistribution : IDegreeDistribution
{
    private int InputSymbolsCount { get; set; }
    private double DecodingFailureProbablity { get; set; }
    private double C { get; set; }

    private readonly List<(int Degree, double Probablity)> ProbabilitesRoulette = new();

    public RobustSolitionDistribution(int intputSymbolsCount, double decodingFailureProbability, double c)
    {
        InputSymbolsCount = intputSymbolsCount;
        DecodingFailureProbablity = decodingFailureProbability;
        C = c;

        var distribution = Enumerable.Range(1, InputSymbolsCount).Select(i => (Degree: i, Probablity: RobustSolitionDistributionValue(i)));
        ProbabilitesRoulette = CumulativeSum(distribution).ToList();
    }

    /// <summary>
    /// Returns a random integer number that can be used as a degree of next symbol to be encoded
    /// </summary>
    /// <returns>Random degree number</returns>
    public int NextDegree()
    {
        return NextDegree(new Random());
    }

    public int NextDegree(Random randomGenerator)
    {
        var randomValue = randomGenerator.NextDouble();

        return ProbabilitesRoulette.First(m => randomValue < m.Probablity).Degree;
    }

    private double R => C * Math.Log(InputSymbolsCount / DecodingFailureProbablity) * Math.Sqrt(InputSymbolsCount);

    private double Tau(int i)
    {
        if (i < 0 || i > InputSymbolsCount)
        {
            throw new ArgumentOutOfRangeException(nameof(i), "Distribution is defined only for values bigger than 1 and smaller than input symbols count");
        }

        var boundary = InputSymbolsCount / R;

        if (i >= 1 && i < boundary)
        {
            return R / (i * InputSymbolsCount);
        }

        if (i == boundary)
        {
            return R * Math.Log(R / DecodingFailureProbablity) / InputSymbolsCount;
        }

        return 0;
    }

    private double IdealSolitionDistributionValue(int i)
    {
        if (i == 1)
        {
            return 1d / InputSymbolsCount;
        }

        return 1d / (i * (i - 1));
    }

    private double NormalizationFactor => Enumerable.Range(1, InputSymbolsCount).Aggregate<int, double>(0, (sum, i) => sum += IdealSolitionDistributionValue(i) + Tau(i));

    private double RobustSolitionDistributionValue(int i) => (Tau(i) + IdealSolitionDistributionValue(i)) / NormalizationFactor;

    private static IEnumerable<(int Degree, double Probability)> CumulativeSum(IEnumerable<(int Degree, double Probability)> probabilites)
    {
        double cumulativeSum = 0;

        foreach (var (Degree, Probability) in probabilites)
        {
            yield return (Degree, cumulativeSum += Probability);
        }
    }
}
