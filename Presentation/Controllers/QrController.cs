using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QrController : ControllerBase
{
    [HttpGet]
    public IActionResult GetQrCode([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return BadRequest("URL parameter is required");

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);

        return File(qrCodeBytes, "image/png");
    }
}