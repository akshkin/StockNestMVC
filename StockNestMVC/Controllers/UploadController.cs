using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StockNestMVC.Data;
using System.Security.Claims;

namespace StockNestMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly Models.MySupabaseOptions _supabase;


    public UploadController(IOptions<Models.MySupabaseOptions> supabase)
    {
        //_supabaseUrl = config["Supabase:Url"];
        //_supabaseServiceRoleKey = config["Supabase:SecretKey"];
        //_context = context;
        _supabase = supabase.Value;
    }

    [HttpGet("upload-image")]
    public async Task<IActionResult> GetSignedUrl()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("user" + userId);
            if (userId == null) return Unauthorized("User not found");

            var fileName = $"{Guid.NewGuid()}.png";
            var filePath = $"{userId}/{fileName}";

            // Supabase client
            var client = new Supabase.Client(_supabase.Url, _supabase.SecretKey);

            // Generate signed upload URL
            var signedUrl = await client.Storage
                .From(_supabase.Bucket)
                .CreateUploadSignedUrl(filePath);

            return Ok(new { signedUrl, filePath, bucket = _supabase.Bucket });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
