using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNestMVC.Interfaces;
using System.Security.Claims;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpGet("upload-image")]
    public async Task<IActionResult> GetSignedUrl()
    {
        var uploadResponse = await _uploadService.GetSignedUrl(User);

        return Ok(uploadResponse);
    }
}
