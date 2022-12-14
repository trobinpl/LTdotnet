using System.Collections;

namespace FountainCodes;

public class EncodedSymbol
{
    public HashSet<int> Neighbours { get; init; }
    public BitArray EncodedData { get; init; }

    public int Degree => Neighbours.Count;

    private EncodedSymbol(BitArray encodedData, HashSet<int> neighbours)
    {
        EncodedData = encodedData;
        Neighbours = neighbours;
    }

    public static EncodedSymbol FromNeighbours(BitArray data, List<(int Index, BitArray Data)> neighbours)
    {
        var encodedData = neighbours.Aggregate(data, (encodedData, neigbbour) => encodedData = encodedData.Xor(neigbbour.Data));

        return new EncodedSymbol(encodedData, neighbours.Select(neighbour => neighbour.Index).ToHashSet());
    }

    public bool IsNeighbourOf(int neighbourIndex) => Neighbours.Contains(neighbourIndex);

    public void RemoveNeighbour(int neighbourIndex)
    {
        Neighbours.Remove(neighbourIndex);
    }
}