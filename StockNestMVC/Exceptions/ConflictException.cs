namespace StockNestMVC.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message, object? data = null) : base(message, 409, data) { }
}
