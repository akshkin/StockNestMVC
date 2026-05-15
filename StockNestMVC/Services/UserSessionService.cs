using Microsoft.AspNetCore.Identity;
using StockNestMVC.DTOs;
using StockNestMVC.Exceptions;
using StockNestMVC.Interfaces;
using StockNestMVC.Models;
using Supabase.Gotrue;
using System.Security.Claims;
using System.Security.Principal;
using UAParser;

namespace StockNestMVC.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepo;
    private readonly GeoService _geoService;
    private readonly UserManager<AppUser> _userManager;
    private static readonly Parser _uaParser = Parser.GetDefault();
    public UserSessionService(IUserSessionRepository userSessionRepo, GeoService geoService, UserManager<AppUser> userManager)
    {
        _userSessionRepo = userSessionRepo;
        _geoService = geoService;
        _userManager = userManager;
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

    public async Task<bool> RevokeSessionAsync(ClaimsPrincipal claimsPrincipal, string refreshToken, int sessionId)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);
        if (user == null)
            throw new UnauthorizedException("User not found");

        var currentSession = await _userSessionRepo.GetSessionByRefreshTokenAsync(refreshToken);

        await _userSessionRepo.RevokeSessionAsync(sessionId, user.Id);

        bool isCurrentSession = currentSession?.UserSessionId == sessionId;

        return isCurrentSession;

        // If user revoked current session → log them out
        //if (currentSession?.UserSessionId == sessionId)
        //{
        //    http.Response.Cookies.Delete("accessToken");
        //    http.Response.Cookies.Delete("refreshToken");
        //}
    }

    public async Task RevokeSessionById(int sessionId, string userId)
    {
        await _userSessionRepo.RevokeSessionAsync(sessionId, userId);

    }

    public async Task<IEnumerable<UserSessionResponseDto>> GetAllSessions(string refreshToken)
    {
        var session = await GetSessionByRefreshTokenAsync(refreshToken);

        if (session == null)
            throw new UnauthorizedAccessException("Invalid session");

        var userId = session.UserId;

        var userSessions = await _userSessionRepo.GetUserSessionsAsync(userId);

        var result = new List<UserSessionResponseDto>();
        var geoCache = new Dictionary<string, (string country, string city)>();

        foreach (var userSession in userSessions)
        {
            var client = _uaParser.Parse(userSession.DeviceName);

            string deviceLabel = $"{client.UA.Family} on {client.OS.Family}";

            string ip = userSession.IpAddress;

            string country = "Unknown";
            string city = "Unknown";

            if (!string.IsNullOrEmpty(ip))
            {
                if (!geoCache.TryGetValue(ip, out var location))
                {
                    location = _geoService.GetLocation(ip);
                    geoCache[ip] = location;
                }

                country = location.country;
                city = location.city;
            }

            result.Add(new UserSessionResponseDto
            {
                SessionId = session.UserSessionId,
                DeviceName = deviceLabel,
                Location = $"{city}, {country}",
                IpAddress = userSession.IpAddress,
                LastActiveAt = userSession.LastActivityAt,
                IsCurrentDevice = userSession.UserSessionId == session.UserSessionId
            });
        }
        return result;
    }
}
