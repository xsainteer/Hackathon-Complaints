using Domain.Entities;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

// [Authorize(Roles = "Admin, SuperAdmin")]
[ApiController]
[Route("api/admin/submissions")]
public class AdminSubmissionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminSubmissionsController> _logger;

    public AdminSubmissionsController(AppDbContext context, ILogger<AdminSubmissionsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpPost("resolve/{id}")]
    public async Task<IActionResult> Resolve(Guid id, string answer = "")
    {
        try
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound("Submission not found");
            }

            submission.Answer = answer;
            submission.Status = SubmissionStatus.Resolved;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();

            return Ok("Submission answered successfully");
        }
        catch (Exception e)
        {
            _logger.LogError("Error answering submission: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPost("reject/{id}")]
    public async Task<IActionResult> Reject(Guid id, string answer = "")
    {
        try
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound("Submission not found");
            }
            
            submission.Answer = answer;
            
            submission.Status = SubmissionStatus.Rejected;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Error rejecting submission: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPost("markasinprogress/{id}")]
    public async Task<IActionResult> MarkAsInProgress(Guid id, string answer = "")
    {
        try
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null)
            {
                return NotFound("Submission not found");
            }
            
            submission.Answer = answer;
            
            submission.Status = SubmissionStatus.InProgress;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync();

            return Ok("Submission marked as in progress");
        }
        catch (Exception e)
        {
            _logger.LogError("Error marking submission as in progress: {Message}", e.Message);
            return StatusCode(500, "Internal server error");
        }
    }
}