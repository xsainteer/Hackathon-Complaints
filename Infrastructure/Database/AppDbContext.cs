using Infrastructure.PostgreSQL.Entities;
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
    
    
}