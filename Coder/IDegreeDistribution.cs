namespace FountainCodes;

public interface IDegreeDistribution
{
    /// <summary>
    /// Returns a random integer number that can be used as a degree of next symbol to be encoded
    /// </summary>
    /// <returns>Random degree number</returns>
    int NextDegree();

    /// <summary>
    /// Returns a random integer number that can be used as a degree of next symbol to be encoded
    /// </summary>
    /// <returns>Random degree number</returns>
    int NextDegree(Random random);
}
