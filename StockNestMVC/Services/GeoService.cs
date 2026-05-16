using IPinfo;
using IPinfo.Models;

namespace StockNestMVC.Services;

public class GeoService
{
    private readonly IPinfoClient _client;

    public GeoService(IConfiguration config)
    {
        var token = config["IPInfo:Token"];

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("IPINFO_TOKEN is missing");

        _client = new IPinfoClient.Builder()
            .AccessToken(token)
            .Build();
    }
    public async Task<(string country, string city)> GetLocation(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return ("Unknown", "Unknown");

        if (ip == "::1" || ip == "127.0.0.1")
            return ("Local", "Localhost");

        try
        {
            var response = await _client.IPApi.GetDetailsAsync(ip);

            return (
                response.Country ?? "Unknown",
                response.City ?? "Unknown"
            );
        }
        catch
        {
            return ("Unknown", "Unknown");
        }
    }
}
