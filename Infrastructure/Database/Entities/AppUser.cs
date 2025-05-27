using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Database.Entities;

public class AppUser : IdentityUser<Guid>
{
    public List<Submission> Complaints { get; set; } = [];
    
}