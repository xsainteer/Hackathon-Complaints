using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class AdminAuthoritiesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminAuthoritiesController> _logger;

    public AdminAuthoritiesController(AppDbContext context, ILogger<AdminAuthoritiesController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    
}