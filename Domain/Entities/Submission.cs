using Domain.Interfaces;

namespace Domain.Entities;

public enum SubmissionType
{
    Complaint,  
    Feedback,  
    Proposal     
}

public class Submission : IHasId, IHasTitle
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public SubmissionType Type { get; set; }
    
    public Guid CreatorId { get; set; }

    public Authority Authority { get; set; } = null!;
    public Guid AuthorityId { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.New;
} 

public enum SubmissionStatus
{
    New,
    InProgress,
    Resolved,
    Rejected
}