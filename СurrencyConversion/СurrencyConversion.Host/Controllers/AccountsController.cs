using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Api.Contracts.Accounts.Responses;
using CurrencyConversion.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace СurrencyConversion.Host.Controllers;

[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsHandler _accountsHandler;

    public AccountsController(IAccountsHandler accountsHandler)
    {
        _accountsHandler = accountsHandler ?? throw new ArgumentNullException(nameof(accountsHandler));
    }
    
    [HttpPost("top-up")]
    public Task<Guid> TopUp([FromBody] TopUpRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _accountsHandler.TopUp(request, token);
    }

    [HttpPost("convert")]
    public Task<ConvertResponse> Convert([FromBody] ConvertRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _accountsHandler.Convert(request, token);
    }
}