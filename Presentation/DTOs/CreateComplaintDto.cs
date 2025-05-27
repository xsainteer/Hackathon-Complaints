namespace Presentation.DTOs;

public class CreateComplaintDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid AuthorityId {get; set; }
}