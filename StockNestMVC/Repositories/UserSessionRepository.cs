using Microsoft.EntityFrameworkCore;
using StockNestMVC.Data;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using Supabase.Gotrue;

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
            .Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.RefreshToken == refreshToken &&
                !s.IsRevoked &&
                s.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<UserSession>> GetUserSessionsAsync(int userId)
        {
            return await _context.UserSessions.Include(s => s.User)
           .Where(s => s.UserId == userId && !s.IsRevoked)
           .OrderByDescending(s => s.LastActivityAt)
           .ToListAsync();
        }

        public async Task RevokeAllUserSessionsAsync(int userId, int? exceptSessionId)
        {
            var sessions = await _context.UserSessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync();

            foreach (var session in sessions)
            {
                if (exceptSessionId == null || session.Id != exceptSessionId)
                {
                    session.IsRevoked = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RevokeSessionAsync(int sessionId)
        {
            var session = await _context.UserSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLastActivityAsync(string refreshToken)
        {
            var session = await _context.UserSessions
           .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);

            if (session != null)
            {
                session.LastActivityAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
