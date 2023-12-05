namespace CurrencyConversion.Api.Contracts.Accounts.Requests;

public class TopUpRequest
{
    public long UserId { get; set; }
    
    public decimal Sum { get; set; }
    
    public long CurrencyId { get; set; }
}