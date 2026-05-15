using StockNestMVC.Models;

namespace StockNestMVC.Interfaces;

public interface IUserSessionService
{
    public Task<UserSession> CreateSessionAsync(string userId, string deviceName, string ipAddress, string refreshToken);

    public Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);

    public Task UpdateLastActivityAsync(UserSession session, string refreshToken);

    public Task RevokeSessionAsync(int sessionId);

}
