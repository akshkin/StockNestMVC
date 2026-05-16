using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;

namespace StockNestMVC.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserSession> CreateSessionAsync(UserSession session)
        {
            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<UserSession?> GetSessionByRefreshTokenAsync(string refreshToken)
        {
            return await _context.UserSessions
            .Include(s => s.AppUser)
            .FirstOrDefaultAsync(s =>
                s.RefreshToken == refreshToken &&
                !s.IsRevoked &&
                s.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<UserSession>> GetUserSessionsAsync(string userId)
        {
            return await _context.UserSessions.Include(s => s.AppUser)
           .Where(s => s.UserId == userId && !s.IsRevoked)
           .OrderByDescending(s => s.LastActivityAt)
           .ToListAsync();
        }

        public async Task RevokeAllUserSessionsAsync(string userId, int? exceptSessionId)
        {
            var sessions = await _context.UserSessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync();

            foreach (var session in sessions)
            {
                if (exceptSessionId == null || session.UserSessionId != exceptSessionId)
                {
                    session.IsRevoked = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevokeSessionAsync(int sessionId, string userId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.UserSessionId == sessionId && s.UserId == userId);

            if (session == null)
                return;

            session.IsRevoked = true;
            await _context.SaveChangesAsync();
        
        }

        public async Task UpdateSession(UserSession session)
        {
            _context.UserSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}
