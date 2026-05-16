
namespace StockNestMVC.Models
{
    public class UserSession
    {
        public int UserSessionId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceName { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public bool IsRevoked { get; set; }
 
    }
}
