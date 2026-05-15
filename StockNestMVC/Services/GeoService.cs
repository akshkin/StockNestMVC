using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;

namespace StockNestMVC.Services;

public class GeoService
{
    private readonly DatabaseReader _reader;

    public GeoService()
    {
        _reader = new DatabaseReader("GeoLite2-City.mmdb");
    }

    public (string country, string city) GetLocation(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return ("Unknown", "Unknown");

        if (ip == "::1" || ip == "127.0.0.1")
            return ("Local", "Localhost");

        try
        {
            var response = _reader.City(ip);

            return (
                response.Country?.Name ?? "Unknown",
                response.City?.Name ?? "Unknown"
            );
        }
        catch (AddressNotFoundException)
        {
            return ("Unknown", "Unknown");
        }
    }
}
