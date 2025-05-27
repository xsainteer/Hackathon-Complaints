using Domain.Entities;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations;

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.HasOne<Authority>(c => c.Authority)
            .WithMany(a => a.Complaints)
            .HasForeignKey(c => c.AuthorityId);
        
        builder.HasOne<AppUser>()
            .WithMany(u => u.Complaints)
            .HasForeignKey(c => c.CreatorId);
    }
}