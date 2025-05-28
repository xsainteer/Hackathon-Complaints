using System.ComponentModel.DataAnnotations;
using Presentation.DTOs.Submission;

namespace Presentation.DTOs.Authority;

public class ReadAuthorityDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<ReadSubmissionDto> ReadSubmissionDtos { get; set; } = new();
}