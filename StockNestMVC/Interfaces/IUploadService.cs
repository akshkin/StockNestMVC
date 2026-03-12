using Microsoft.AspNetCore.Mvc;
using StockNestMVC.DTOs;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IUploadService
{
    public Task<UploadResponseDto> GetSignedUrl(ClaimsPrincipal claimsPrincipal);

}
