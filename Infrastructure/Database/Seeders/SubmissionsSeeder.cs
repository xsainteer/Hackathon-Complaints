using Bogus;
using Domain.Entities;

namespace Infrastructure.Database.Seeders;

// Seed before app.Run() and before app.Build() in Program.cs
public static class SubmissionsSeeder
{
    public static void SeedRandomSubmissions(AppDbContext context)
    {
        // if any submissions already exist, skip seeding
        if (context.Submissions.Any()) return;
        
        var authorityIds = context.Authorities.Select(a => a.Id).ToList();

        if (authorityIds.Count == 0)
        {
            throw new InvalidOperationException("No authorities found. Please seed authorities first.");
        }
        
        var submissionFaker = new Faker<Submission>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Title, f => f.Lorem.Sentence())
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph())
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1))
            .RuleFor(s => s.Type, f => f.PickRandom<SubmissionType>())
            .RuleFor(s => s.CreatorId, f => Guid.NewGuid())
            .RuleFor(s => s.Status, f => f.PickRandom<SubmissionStatus>())
            .RuleFor(s => s.Answer, f => f.Lorem.Sentence())
            .RuleFor(s => s.Location, f => new GeoPoint(
                f.Address.Latitude(),
                f.Address.Longitude()
            ))
            .RuleFor(s => s.AuthorityId, f => f.PickRandom(authorityIds));

        var mockSubmissions = submissionFaker.Generate(200);
        
        context.Submissions.AddRange(mockSubmissions);

        context.SaveChanges();
    }
}