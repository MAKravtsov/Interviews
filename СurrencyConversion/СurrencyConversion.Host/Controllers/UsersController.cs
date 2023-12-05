using CurrencyConversion.Api.Contracts.Users.Requests;
using CurrencyConversion.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace СurrencyConversion.Host.Controllers;

[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUsersHandler _usersHandler;

    public UsersController(IUsersHandler usersHandler)
    {
        _usersHandler = usersHandler ?? throw new ArgumentNullException(nameof(usersHandler));
    }

    [HttpPost]
    public Task<long> AddUser([FromBody] AddUserRequest request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return _usersHandler.AddUser(request, token);
    }
}