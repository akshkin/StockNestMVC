using StockNestMVC.Models;

namespace StockNestMVC.Interfaces
{
    public interface IUserSessionRepository
    {
        public Task<UserSession> CreateSessionAsync(UserSession session);

        public Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);

        public Task<List<UserSession>> GetUserSessionsAsync(int userId);

        public Task RevokeSessionAsync(int sessionId);

        public Task RevokeAllUserSessionsAsync(int userId, int? exceptSessionId = null);

        public Task UpdateLastActivityAsync(string refreshToken);
    }
}
