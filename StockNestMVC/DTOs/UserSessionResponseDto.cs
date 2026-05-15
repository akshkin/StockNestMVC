namespace StockNestMVC.DTOs;

public class UserSessionResponseDto
{
    public int SessionId { get; set; }
    public string DeviceName { get; set; }// "Chrome on Windows",
    public string Location { get; set; }// "Stockholm, Sweden",
    public string IpAddress { get; set; } // "127.0.0.1", 
    public DateTime LastActiveAt { get; set; } // "2026-05-15T10:00:00Z",
    public bool IsCurrentDevice { get; set; } // true
}

