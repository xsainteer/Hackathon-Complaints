using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs.Authority;

public class CreateAuthorityDto
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}