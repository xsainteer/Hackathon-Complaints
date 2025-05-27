using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthoritiesController : GenericController<Authority>
{
    public AuthoritiesController(IGenericService<Authority> service, ILogger<GenericController<Authority>> logger) : base(service, logger)
    {
    }
}