using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepo;
    public UserSessionService(IUserSessionRepository userSessionRepo)
    {
        _userSessionRepo = userSessionRepo;
    }

    public async Task<UserSession> CreateSessionAsync(string userId, string deviceName, string ipAddress, string refreshToken)
    {
        var session = new UserSession
        {
            UserId = userId,
            RefreshToken = refreshToken,
            DeviceName = deviceName,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            LastActivityAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _userSessionRepo.CreateSessionAsync(session);
        return session;
    }

    public async Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken)
    {
        var sessionByRefreshToken = await _userSessionRepo.GetSessionByRefreshTokenAsync(refreshToken);

        if (sessionByRefreshToken == null)
            return null;

        return sessionByRefreshToken;
    }

    public async Task UpdateLastActivityAsync(UserSession session, string refreshToken)
    {
        session.RefreshToken = refreshToken;
        await _userSessionRepo.UpdateSession(session);
    }

    public async Task RevokeSessionAsync(int sessionId)
    {
        await _userSessionRepo.RevokeSessionAsync(sessionId);
    }
}
