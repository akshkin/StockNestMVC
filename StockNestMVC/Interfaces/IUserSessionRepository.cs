using StockNestMVC.Models;

namespace StockNestMVC.Interfaces
{
    public interface IUserSessionRepository
    {
        public Task<UserSession> CreateSessionAsync(UserSession session);

        public Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken);

        public Task<List<UserSession>> GetUserSessionsAsync(string userId);

        public Task RevokeSessionAsync(int sessionId, string userId);

        public Task RevokeAllUserSessionsAsync(string userId, int? exceptSessionId = null);

        public Task UpdateSession(UserSession session);

    }
}
