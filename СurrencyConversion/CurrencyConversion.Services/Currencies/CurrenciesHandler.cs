using CurrencyConversion.Api.Contracts.Currencies.Requests;
using CurrencyConversion.Data.Contexts;
using CurrencyConversion.Data.Entities;

namespace CurrencyConversion.Services.Currencies;

internal class CurrenciesHandler : ICurrenciesHandler
{
    private readonly DatabaseContext _context;

    public CurrenciesHandler(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<long> AddCurrency(AddCurrencyRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        var currency = new CurrencyEntity
        {
            Name = request.Name,
        };

        await _context.Currencies.AddAsync(currency, token);
        await _context.SaveChangesAsync(token);

        return currency.Id;
    }
}