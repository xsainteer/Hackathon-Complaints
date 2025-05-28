using Qdrant.Client.Grpc;

namespace Infrastructure.AI.Vectors;

public class QDrantSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public int VectorSize { get; set; }
    public Distance Distance { get; set; } = Distance.Cosine;
    
    public float SimilarityThreshold { get; set; }
}