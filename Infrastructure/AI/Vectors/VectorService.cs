using Infrastructure.AI.Ollama;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Infrastructure.AI.Vectors;

public class VectorService
{
    private readonly QdrantClient _qdrantClient;
    private readonly ILogger<VectorService> _logger;
    private readonly QDrantSettings _qDrantSettings;
    private readonly OllamaClient _ollamaClient;
    
    public VectorService(QdrantClient qdrantClient, ILogger<VectorService> logger, IOptions<QDrantSettings> qDrantSettings, OllamaClient ollamaClient)
    {
        _qdrantClient = qdrantClient;
        _logger = logger;
        _ollamaClient = ollamaClient;
        _qDrantSettings = qDrantSettings.Value;
    }

    public async Task EnsureCollectionExistsAsync()
    {
        try
        {
            var collectionExists = await _qdrantClient.CollectionExistsAsync(_qDrantSettings.CollectionName);
            if (!collectionExists)
            {
                var vectorParams = new VectorParams 
                { 
                    Size = (ulong)_qDrantSettings.VectorSize,
                    Distance = _qDrantSettings.Distance,
                    OnDisk = false
                };
                
                await _qdrantClient.CreateCollectionAsync(
                    _qDrantSettings.CollectionName,
                    vectorParams);
                
                _logger.LogInformation("Created Qdrant collection {CollectionName}", _qDrantSettings.CollectionName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while ensuring the collection exists.");
            throw;  
        }
    }

    public async Task IndexSubmissionAsync(Guid submissionId, string shortDescription)
    {
        try
        {
            await EnsureCollectionExistsAsync();
            
            var embedding = await _ollamaClient.GenerateEmbeddingAsync(shortDescription);
            
            if (embedding == null || embedding.Length != _qDrantSettings.VectorSize)
            {
                throw new InvalidOperationException("Invalid embedding generated.");
            }
            
            var point = new PointStruct
            {
                Id = new PointId(submissionId),
                Vectors = embedding.ToArray()
            };
            
            await _qdrantClient.UpsertAsync(
                _qDrantSettings.CollectionName,
                new[] { point });
            
            _logger.LogInformation("Indexed submission {Id} in Qdrant", submissionId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    // public async Task<List<CommonProblemDto>> GetCommonProblemsAsync(int top = 10)
    // {
    //     var allPoints = await _qdrantClient.SearchAsync(
    //         _qDrantSettings.CollectionName,
    //         
    //         );
    // }
    
    public static float CosineSimilarity(IEnumerable<float> v1, IEnumerable<float> v2)
    {
        var vec1 = v1.ToArray();
        var vec2 = v2.ToArray();

        float dot = 0, mag1 = 0, mag2 = 0;

        for (int i = 0; i < vec1.Length; i++)
        {
            dot += vec1[i] * vec2[i];
            mag1 += vec1[i] * vec1[i];
            mag2 += vec2[i] * vec2[i];
        }

        return dot / ((float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2));
    }
    
    
    // public async Task<List<RetrievedPoint>> GetAllPointsAsync()
    // {
    //     var allPoints = new List<RetrievedPoint>();
    //     PointId? nextOffset = null;
    //
    //     do
    //     {
    //         var response = await _qdrantClient.ScrollAsync(
    //             collectionName : _qDrantSettings.CollectionName,
    //             limit : 100, // можно менять, максимум — зависит от настроек сервера
    //             offset : nextOffset
    //         );
    //
    //         allPoints.AddRange(response.Result);
    //
    //         offset = response.NextPageOffset?.ToString();
    //
    //     } while (offset != null);
    //
    //     return allPoints;
    // }

}