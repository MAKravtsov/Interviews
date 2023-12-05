namespace CurrencyConversion.Api.Contracts.Accounts.Requests;

public class ConvertRequest
{
    public long UserId { get; set; }
    
    public long CurrencyIdFrom { get; set; }
    
    public long CurrencyIdTo { get; set; }
    
    public decimal SumFrom { get; set; }
    
    public decimal Rate { get; set; }

    public double? CommissionPercent { get; set; }
}