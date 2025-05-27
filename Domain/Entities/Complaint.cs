namespace Domain.Entities;

public class Complaint
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public Guid CreatorId { get; set; }

    public Authority Authority { get; set; } = null!;
    public Guid AuthorityId { get; set; }
} 