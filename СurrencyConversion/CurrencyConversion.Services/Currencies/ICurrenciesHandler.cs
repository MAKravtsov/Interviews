using CurrencyConversion.Api.Contracts.Currencies.Requests;

namespace CurrencyConversion.Services.Currencies;

public interface ICurrenciesHandler
{
    Task<long> AddCurrency(AddCurrencyRequest request, CancellationToken token);
}