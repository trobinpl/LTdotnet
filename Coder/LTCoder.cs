using System.Collections;

namespace FountainCodes;

/// <summary>
/// Provides a method to encode data stream using LT code
/// </summary>
public class LTCoder
{
    public int EncodingSymbolLength { get; private set; }
    public IDegreeDistribution Distribution { get; private set; }
    public Stream DataStream { get; private set; }

    private int ChunksCount { get; set; }

    public LTCoder(Stream dataStream, int encodingSymbolLength, IDegreeDistribution distribution)
    {
        DataStream = dataStream;
        EncodingSymbolLength = encodingSymbolLength;
        Distribution = distribution;
        ChunksCount = (int)Math.Ceiling((double)dataStream.Length / EncodingSymbolLength);
    }

    /// <summary>
    /// Using the provided stream it will produce infinite amount of encoded symbols. Use .Take() method to get specified amount of encoded symbols
    /// </summary>
    /// <param name="stream">Stream of data to encode</param>
    /// <returns></returns>
    public IEnumerable<EncodedSymbol> Encode(Stream stream)
    {
        using var streamReader = new BinaryReader(stream);
        for (int i = 0; true; i++)
        {
            var currentChunkIndex = i % ChunksCount;

            var degree = Distribution.NextDegree();
            var currentData = GetStreamChunk(currentChunkIndex, streamReader);
            var neighbours = GetNeighbours(degree, streamReader);
            yield return EncodedSymbol.FromNeighbours(currentData, neighbours);
        }
    }

    private List<(int ChunkIndex, BitArray Data)> GetNeighbours(int degree, BinaryReader streamReader)
    {
        var generator = new Random();
        var chunks = new List<(int ChunkIndex, BitArray Data)>();

        for (int i = 0; i < degree; i++)
        {
            var randomChunkNumber = generator.Next(ChunksCount);
            var chunkData = GetStreamChunk(randomChunkNumber, streamReader);

            chunks.Add((randomChunkNumber, chunkData));
        }

        return chunks;
    }

    private BitArray GetStreamChunk(int index, BinaryReader streamReader)
    {
        streamReader.BaseStream.Seek(index * EncodingSymbolLength, SeekOrigin.Begin);

        var bytes = streamReader.ReadBytes(EncodingSymbolLength);

        var missingBytes = EncodingSymbolLength - bytes.Length;

        if (missingBytes == 0)
        {
            return new BitArray(bytes);
        }

        var bytesList = bytes.ToList();
        bytesList.AddRange(Enumerable.Range(1, missingBytes).Select(_ => (byte)0xFF));

        return new BitArray(bytesList.ToArray());
    }
}