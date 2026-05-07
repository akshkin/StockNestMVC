namespace StockNestMVC.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public object? DataPayload { get;  }

    protected AppException(string message, int statusCode, object? dataPayload = null) : base(message)
    {
        StatusCode = statusCode;
        DataPayload = dataPayload;
    } 
}
