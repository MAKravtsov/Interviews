namespace CurrencyConversion.Services.Common.Exceptions;

public class AlertException : Exception
{
    public AlertException(string message) : base(message)
    {
    }
}