using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QrController : ControllerBase
{
    private readonly AppDbContext _context;

    public QrController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetQrCode([FromQuery] string url,[FromQuery] Guid authrorityId)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL parameter is required");
        
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);

        var auth = await _context.Authorities.FirstAsync(a => a.Id == authrorityId);

        auth.Branches.Add(url);
        
        _context.Authorities.Update(auth);

        await _context.SaveChangesAsync();

        return File(qrCodeBytes, "image/png");
    }
}