using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.AI;
using Infrastructure.AI.Ollama;
using Infrastructure.AI.Vectors;
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
    private readonly OllamaClient _ollamaClient;
    private readonly VectorService _vectorService;

    public SubmissionsController(IGenericService<Submission> service, ILogger<GenericController<Submission, CreateSubmissionDto, ReadSubmissionDto, UpdateSubmissionDto>> logger, IMapper mapper, AppDbContext context, EmailService emailService, OllamaClient ollamaClient, VectorService vectorService) : base(service, logger, mapper)
    {
        _context = context;
        _emailService = emailService;
        _ollamaClient = ollamaClient;
        _vectorService = vectorService;
    }

    public override async Task<IActionResult> Add(CreateSubmissionDto createDto)
    {
        try
        {
            // making a short description using AI
            var shortDescription = await _ollamaClient.MakeShortDescriptionAsync(createDto.Description);
            
            if(shortDescription?.Trim() == "Неадекватно.")
            {
                return BadRequest("Submission is deemed inadequate by AI. Please revise your submission.");
            }
            
            // creating a new GUID so vector db and Postgres Ids are alike
            var id = Guid.NewGuid();
            
            // embedding the short description into vector database
            await _vectorService.IndexSubmissionAsync(id, shortDescription ?? "No short description generated");
            
            
            // overriding so when submission submitted its sent to Authority directly
            var to = await _context
                .Authorities
                .FindAsync(createDto.AuthorityId)
                ?? throw new Exception("Authority not found");
            
            await _emailService.SendEmailAsync(
                to.Email,
                $"{createDto.SubmissionType.ToString()}, {createDto.Title}",
                $"{createDto.Description}\nLocation: {createDto.Location?.To2GisUrl() ?? "No location provided"}");

            var submission = _mapper.Map<Submission>(createDto);
            submission.Id = id;
            submission.ShortDescription = shortDescription ?? "No short description generated";
            
            await _service.AddAsync(submission);
            return Ok();
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