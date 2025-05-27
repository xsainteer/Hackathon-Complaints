using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Presentation.DTOs.Authority;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthoritiesController : GenericController<Authority, CreateAuthorityDto, ReadAuthorityDto, UpdateAuthorityDto>
{
    public AuthoritiesController(IGenericService<Authority> service, ILogger<GenericController<Authority, CreateAuthorityDto, ReadAuthorityDto, UpdateAuthorityDto>> logger, IMapper mapper) : base(service, logger, mapper)
    {
    }
}