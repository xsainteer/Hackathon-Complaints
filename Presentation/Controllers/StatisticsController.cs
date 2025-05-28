using Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin,SuperAdmin")]
[ApiController]
[Route("[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(AppDbContext context, ILogger<StatisticsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    
}