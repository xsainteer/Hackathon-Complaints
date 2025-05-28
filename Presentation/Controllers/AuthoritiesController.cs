using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.DTOs.Authority;

namespace Presentation.Controllers;

// [Authorize(Roles = "Admin,SuperAdmin")]
[ApiController]
[Route("/api/[controller]")]
public class AuthoritiesController : GenericController<Authority, CreateAuthorityDto, ReadAuthorityDto, UpdateAuthorityDto>
{
    private readonly AppDbContext _context;
    public AuthoritiesController(IGenericService<Authority> service, ILogger<GenericController<Authority, CreateAuthorityDto, ReadAuthorityDto, UpdateAuthorityDto>> logger, IMapper mapper, AppDbContext context) : base(service, logger, mapper)
    {
        _context = context;
    }

    public override async Task<IActionResult> GetAll(int skip = 0, int count = 10, string query = "")
    {
        try
        {
            var authoritiesWithSubs = await _context.Authorities
                .Skip(skip)
                .Take(count)
                .Include(a => a.Submissions)
                .ToListAsync();

            var dtos = authoritiesWithSubs
                .Select(x => _mapper.Map<ReadAuthorityDto>(x));

            return Ok(dtos);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while fetching authorities");
            return StatusCode(500, new { message = "An error occurred while fetching authorities" });
        }
    }

    public override async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var authorityWithSubs = await _context
                .Authorities
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            var dto = _mapper.Map<ReadAuthorityDto>(authorityWithSubs);
            
            return Ok(dto);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while fetching authority with id {Id}", id);
            throw;
        }
    }
}