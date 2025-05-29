using Domain.Entities;
using Infrastructure.AI.Ollama;
using Infrastructure.AI.Vectors;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Presentation.DTOs.Submission;
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
    private readonly OllamaClient _ollamaClient;

    public StatisticsController(QdrantClient qdrantClient, VectorService vectorService, ILogger<StatisticsController> logger, AppDbContext context, IOptions<QDrantSettings> qDrantSettings, OllamaClient ollamaClient)
    {
        _qdrantClient = qdrantClient;
        _vectorService = vectorService;
        _logger = logger;
        _context = context;
        _ollamaClient = ollamaClient;
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


    [HttpGet("semantic-search")]
    public async Task<IActionResult> SearchSubmissions([FromQuery] string query = "")
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Query string cannot be empty" });

        try
        {
            var queryVector = await _ollamaClient.GenerateEmbeddingAsync(query);

            var results = await _qdrantClient.SearchAsync(
                collectionName: _qDrantSettings.CollectionName,
                queryVector.ToArray(),
                limit: 1000,
                filter: new Filter());

            var passedResults = results
                .Where(sp => sp.Score >= _qDrantSettings.SimilarityThreshold)
                .ToList();

            var scoreDict = passedResults.ToDictionary(r => Guid.Parse(r.Id.Uuid), r => r.Score);

            var passedResultIds = scoreDict.Keys.ToList();

            var submissions = await _context.Submissions
                .Where(s => passedResultIds.Contains(s.Id))
                .AsNoTracking()
                .Select(s => new ReadSubmissionDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    ShortDescription = s.ShortDescription,
                    CreatedAt = s.CreatedAt,
                    CreatorId = s.CreatorId,
                    AuthorityId = s.AuthorityId,
                    SubmissionType = s.Type,
                    Status = s.Status,
                    Answer = s.Answer,
                    Location = s.Location
                })
                .ToListAsync();

            var resultWithScores = submissions
                .Select(s => new ScoredSubmissionDto
                {
                    Submission = s,
                    Score = scoreDict[s.Id]
                })
                .OrderByDescending(s => s.Score)
                .ToList();
            
            var resultDto = new ResultDto
            {
                Submissions = resultWithScores,
                TotalCount = resultWithScores.Count
            };

            return Ok(resultDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while searching submissions.");
            throw;
        }
    }


    private class ResultDto
    {
        public List<ScoredSubmissionDto> Submissions { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public class ScoredSubmissionDto
    {
        public ReadSubmissionDto Submission { get; set; } = null!;
        public float Score { get; set; }
    }
}