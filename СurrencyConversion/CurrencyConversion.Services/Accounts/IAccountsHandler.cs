using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Api.Contracts.Accounts.Responses;

namespace CurrencyConversion.Services.Accounts;

public interface IAccountsHandler
{
    Task<Guid> TopUp(TopUpRequest request, CancellationToken token);

    Task<ConvertResponse> Convert(ConvertRequest request, CancellationToken token);
}