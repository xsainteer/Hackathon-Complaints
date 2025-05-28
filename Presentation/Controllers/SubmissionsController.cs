using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.DTOs.Submission;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SubmissionsController : GenericController<Submission, CreateSubmissionDto, ReadSubmissionDto, UpdateSubmissionDto>
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;
    public SubmissionsController(IGenericService<Submission> service, ILogger<GenericController<Submission, CreateSubmissionDto, ReadSubmissionDto, UpdateSubmissionDto>> logger, IMapper mapper, AppDbContext context, EmailService emailService) : base(service, logger, mapper)
    {
        _context = context;
        _emailService = emailService;
    }

    public override async Task<IActionResult> Add(CreateSubmissionDto createDto)
    {
        try
        {
            // overriding so when submission submitted its sent to Authority directly
            var to = await _context
                .Authorities
                .FindAsync(createDto.AuthorityId)
                ?? throw new Exception("Authority not found");
            
            await _emailService.SendEmailAsync(
                to.Email,
                $"{createDto.SubmissionType.ToString()}, {createDto.Title}",
                $"{createDto.Description}\nLocation: {createDto.Location?.To2GisUrl() ?? "No location provided"}");

            return await base.Add(createDto);
        }
        catch (Exception e)
        {
            _logger.LogError("Error sending submission email: {Message}", e.Message);
            throw;
        }
    }

    [HttpGet("byauthority/{id}")]
    public async Task<IActionResult> GetByAuthorityId(Guid id, [FromQuery] int skip = 0, [FromQuery] int take = 0)
    {
        try
        {
            var result = await _context.Submissions
                .Where(s => s.AuthorityId == id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            var readDtos = result.Select(x => _mapper.Map<ReadSubmissionDto>(x));
            
            return Ok(readDtos);
        }
        catch (Exception e)
        {
            _logger.LogError("Error fetching submissions by authority: {Message}", e.Message);
            throw;
        }
    }
}