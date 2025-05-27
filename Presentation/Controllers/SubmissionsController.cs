using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.DTOs.Submission;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SubmissionsController : GenericController<Submission, CreateSubmissionDto, ReadSubmissionDto, UpdateSubmissionDto>
{
    private readonly AppDbContext _context;
    public SubmissionsController(IGenericService<Submission> service, ILogger<GenericController<Submission, CreateSubmissionDto, ReadSubmissionDto, UpdateSubmissionDto>> logger, IMapper mapper, AppDbContext context) : base(service, logger, mapper)
    {
        _context = context;
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
            Console.WriteLine(e);
            throw;
        }
    }
}