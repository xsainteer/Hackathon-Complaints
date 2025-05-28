using Bogus;
using Domain.Entities;

namespace Infrastructure.Database.Seeders;

// Seed before app.Run() and before app.Build() in Program.cs
public static class AuthoritiesSeeder
{
    public static void SeedKyrgyzAuthorities(this AppDbContext context)
    {
        var kyrgyzAuthorities = new List<Authority>
        {
            new Authority
            {
                Id = Guid.NewGuid(),
                Title = "Министерство цифрового развития Кыргызской Республики",
                Url = "https://digital.gov.kg",
                Email = "info@digital.gov.kg",
                Submissions = new List<Submission>()
            },
            new Authority
            {
                Id = Guid.NewGuid(),
                Title = "Государственная налоговая служба при Министерстве финансов КР",
                Url = "https://www.sti.gov.kg",
                Email = "info@sti.gov.kg",
                Submissions = new List<Submission>()
            },
            new Authority
            {
                Id = Guid.NewGuid(),
                Title = "Министерство образования и науки Кыргызской Республики",
                Url = "https://edu.gov.kg",
                Email = "contact@edu.gov.kg",
                Submissions = new List<Submission>()
            },
            new Authority
            {
                Id = Guid.NewGuid(),
                Title = "Министерство внутренних дел Кыргызской Республики",
                Url = "https://mvd.gov.kg",
                Email = "info@mvd.gov.kg",
                Submissions = new List<Submission>()
            },
            new Authority
            {
                Id = Guid.NewGuid(),
                Title = "Государственная регистрационная служба при Кабинете Министров КР",
                Url = "https://grs.gov.kg",
                Email = "support@grs.gov.kg",
                Submissions = new List<Submission>()
            }
        };

        context.AddRange(kyrgyzAuthorities);
        context.SaveChanges();
    }
    
    public static void SeedRandomAuthorities(this AppDbContext context)
    {
        // if any authorities already exist, skip seeding
        if (context.Authorities.Any()) return;

        var authorityFaker = new Faker<Authority>()
            .RuleFor(a => a.Id, f => Guid.NewGuid())
            .RuleFor(a => a.Title, f => f.Company.CompanyName())
            .RuleFor(a => a.Url, f => f.Internet.Url())
            .RuleFor(a => a.Email, f => f.Internet.Email());

        var mockAuthorities = authorityFaker.Generate(10);

        context.Authorities.AddRange(mockAuthorities);
        context.SaveChanges();
    }
}