using Domain.Entities;
using Infrastructure.Database.Configurations;
using Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    // For EF
    public AppDbContext() {}
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Authority> Authorities { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ComplaintConfiguration());
        builder.ApplyConfiguration(new AuthorityConfiguration());
        builder.ApplyConfiguration(new AppUserConfiguration());
        
        base.OnModelCreating(builder);
    }
}