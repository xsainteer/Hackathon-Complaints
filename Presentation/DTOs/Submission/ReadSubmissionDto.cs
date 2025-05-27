using Domain.Entities;

namespace Presentation.DTOs.Submission;

public class ReadSubmissionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatorId { get; set; }
    public Guid AuthorityId { get; set; }
    public SubmissionType SubmissionType { get; set; }
    public SubmissionStatus Status { get; set; }
    public GeoPoint? Location { get; set; }
}