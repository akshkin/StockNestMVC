using StockNestMVC.DTOs;
using StockNestMVC.Models;
using System.Security.Claims;

namespace StockNestMVC.Interfaces;

public interface IUserSessionService
{
    public Task<UserSession> CreateSessionAsync(string userId, string deviceName, string ipAddress, string refreshToken);

    public Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);

    public Task UpdateLastActivityAsync(UserSession session, string refreshToken);

    public Task<bool> RevokeSessionAsync(ClaimsPrincipal claimsPrincipal, string refreshToken, int sessionId);

    public Task RevokeSessionById(int sessionId, string userId);

    public Task<IEnumerable<UserSessionResponseDto>> GetAllSessions(string refreshToken);

}
