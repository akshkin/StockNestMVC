using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StockNestMVC.DTOs;
using StockNestMVC.Exceptions;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using Supabase.Gotrue;
using System.Security.Claims;

namespace StockNestMVC.Services;

public class UploadService : IUploadService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly Models.MySupabaseOptions _supabase;

    public UploadService(UserManager<AppUser> userManager, IOptions<Models.MySupabaseOptions> supabase)
    {
        _userManager = userManager;
        _supabase = supabase.Value;
    }

    public async Task<UploadResponseDto> GetSignedUrl(ClaimsPrincipal claimsPrincipal)
    {
        var userId = await _userManager.GetUserAsync(claimsPrincipal);

        if (userId == null) throw new UnauthorizedException("User not found");

        var fileName = $"{Guid.NewGuid()}.png";
        var filePath = $"{userId}/{fileName}";

        // Supabase client
        var client = new Supabase.Client(_supabase.Url, _supabase.SecretKey);

        // Generate signed upload URL
        var signedUrl = await client.Storage
            .From(_supabase.Bucket)
            .CreateUploadSignedUrl(filePath);

        return new UploadResponseDto
        {
            SignedUrl = signedUrl,
            FilePath = filePath,
            Bucket = _supabase.Bucket 
        };

    }
}
