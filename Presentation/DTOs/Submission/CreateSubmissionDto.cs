using Domain.Entities;

namespace Presentation.DTOs.Submission;

public class CreateSubmissionDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
    public Guid AuthorityId {get; set; }
    public SubmissionType SubmissionType { get; set; }
    public GeoPoint? Location { get; set; }
}