using System.Security.Claims;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ComplaintsController : GenericController<Complaint>
{
    
    public ComplaintsController(IGenericService<Complaint> service, ILogger<GenericController<Complaint>> logger) : base(service, logger)
    {
    }
    
    // private readonly IGenericService<Complaint> _complaintService;
    // private readonly ILogger<ComplaintsController> _logger;
    // private readonly IMapper _mapper;
    //
    // public ComplaintsController(IGenericService<Complaint> complaintService, ILogger<ComplaintsController> logger, IMapper mapper)
    // {
    //     _complaintService = complaintService;
    //     _logger = logger;
    //     _mapper = mapper;
    // }

    // [Authorize]
    // [HttpPost]
    // public async Task<IActionResult> CreateComplaint(CreateComplaintDto dto)
    // {
    //     try
    //     {
    //         // Getting the user from HttpContext
    //         var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //         
    //         if (userIdString is null || !Guid.TryParse(userIdString, out var userId))
    //             return Unauthorized();
    //
    //         var complaint = _mapper.Map<Complaint>(dto);
    //         
    //         complaint.CreatorId = userId;
    //         
    //         await _complaintService.AddAsync(complaint);
    //         
    //         return Ok();
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError("error creating complaint: {ErrorMessage}", e.Message);
    //         throw;
    //     }
    // }
    //
    // [Authorize]
    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdateComplaint(Complaint complaint, string id)
    // {
    //     try
    //     {
    //         await _complaintService.UpdateAsync(complaint);
    //         return Ok();
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError("error updating complaint: {ErrorMessage}", e.Message);
    //         throw;
    //     }
    // }
}