using Domain.Entities;
using Infrastructure.AI.Vectors;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Presentation.Controllers;

// [Authorize(Roles = "Admin,SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StatisticsController> _logger;
    private readonly QdrantClient _qdrantClient;
    private readonly VectorService _vectorService;
    private readonly QDrantSettings _qDrantSettings;

    public StatisticsController(QdrantClient qdrantClient, VectorService vectorService, ILogger<StatisticsController> logger, AppDbContext context, IOptions<QDrantSettings> qDrantSettings)
    {
        _qdrantClient = qdrantClient;
        _vectorService = vectorService;
        _logger = logger;
        _context = context;
        _qDrantSettings = qDrantSettings.Value;
    }


    [HttpGet("common-problems")]
    public async Task<IActionResult> GetCommonProblemsAsync()
    {
        try
        {
            await _vectorService.EnsureCollectionExistsAsync();
            var submissions = await _context.Submissions.ToListAsync();
            var ids = submissions.Select(s => s.Id).ToList();
            
            var points = await _qdrantClient.RetrieveAsync(
                collectionName: _qDrantSettings.CollectionName,
                ids: ids.Select(id => new PointId(id)).ToList(),
                withVectors: true,
                withPayload: true);
            
            var allVectors = points.Select(p => p.Vectors).ToList();
            
            var used = new bool[allVectors.Count];
            
            var results = new List<CommonProblemDto>();
            
            for (int i = 0; i < allVectors.Count; i++)
            {
                if (used[i]) continue;

                var currentGroup = new List<Submission> { submissions[i] };
                used[i] = true;

                for (int j = i + 1; j < allVectors.Count; j++)
                {
                    if (used[j]) continue;

                    var sim = VectorService.CosineSimilarity(allVectors[i].Vector.Data, allVectors[j].Vector.Data);
                    if (sim >= _qDrantSettings.SimilarityThreshold)
                    {
                        currentGroup.Add(submissions[j]);
                        used[j] = true;
                    }
                }

                results.Add(new CommonProblemDto
                {
                    RepresentativeDescription = submissions[i].ShortDescription,
                    Occurrences = currentGroup.Count,
                    SimilarSubmissions = currentGroup.Select(s => new SimilarSubmission
                    {
                        SubmissionId = s.Id,
                        ShortDescription = s.ShortDescription
                    }).ToList()
                });
            }
            
            
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving common problems.");
            return StatusCode(500, "Internal server error");
        }
    }

}