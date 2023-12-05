using CurrencyConversion.Api.Contracts.Currencies.Requests;
using CurrencyConversion.Services.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace СurrencyConversion.Host.Controllers;

[Route("currencies")]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrenciesHandler _currenciesHandler;

    public CurrenciesController(ICurrenciesHandler currenciesHandler)
    {
        _currenciesHandler = currenciesHandler ?? throw new ArgumentNullException(nameof(currenciesHandler));
    }
    
    [HttpPost]
    public Task<long> AddCurrency([FromBody] AddCurrencyRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _currenciesHandler.AddCurrency(request, token);
    }
}