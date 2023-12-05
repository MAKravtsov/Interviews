using CurrencyConversion.Api.Contracts.Users.Requests;

namespace CurrencyConversion.Services.Users;

public interface IUsersHandler
{
    Task<long> AddUser(AddUserRequest request, CancellationToken token);
}