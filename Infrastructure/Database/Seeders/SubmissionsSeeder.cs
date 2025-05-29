using Bogus;
using Domain.Entities;
using Infrastructure.AI.Ollama;
using Infrastructure.AI.Vectors;

namespace Infrastructure.Database.Seeders;

// Seed before app.Run() and before app.Build() in Program.cs
public class SubmissionsSeeder
{
    private readonly VectorService _vectorService;
    private readonly OllamaClient _ollamaClient;

    public SubmissionsSeeder(VectorService vectorService, OllamaClient ollamaClient)
    {
        _vectorService = vectorService;
        _ollamaClient = ollamaClient;
    }

    public async Task SeedRandomSubmissions(AppDbContext context)
    {
        var authorityIds = context.Authorities.Select(a => a.Id).ToList();
        
        var creatorIds = context.Users.Select(u => u.Id).ToList();

        if (authorityIds.Count == 0)
        {
            throw new InvalidOperationException("No authorities found. Please seed authorities first.");
        }

        var submissionFaker = new Faker<Submission>("ru")
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Title, f => f.Lorem.Sentence())
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph())
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(s => s.Type, f => f.PickRandom<SubmissionType>())
            .RuleFor(s => s.Location, f => new GeoPoint(
                f.Address.Latitude(),
                f.Address.Longitude()
            ))
            .RuleFor(s => s.AuthorityId, f => f.PickRandom(authorityIds))
            .RuleFor(s => s.CreatorId, f => f.PickRandom(creatorIds));

        var mockSubmissions = submissionFaker.Generate(200);

        // Generate short descriptions using AI
        foreach (var ms in mockSubmissions)
        {
            ms.ShortDescription = await _ollamaClient.MakeShortDescriptionForSeedsAsync(ms.Description);
        }
        
        // Prepare data for vector database
        var submissionsForVdb = mockSubmissions.Select(ms => new
        {
            ms.Id,
            ms.Description
        });
        
        // Indexing submissions in vector database
        foreach (var submission in submissionsForVdb)
        {
            await _vectorService.IndexSubmissionAsync(submission.Id, submission.Description);
        }
        
        await context.Submissions.AddRangeAsync(mockSubmissions);

        await context.SaveChangesAsync();
    }
}