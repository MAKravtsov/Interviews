using CurrencyConversion.Api.Contracts.Users.Requests;
using CurrencyConversion.Data.Contexts;
using CurrencyConversion.Data.Entities;

namespace CurrencyConversion.Services.Users;

internal class UsersHandler : IUsersHandler
{
    private readonly DatabaseContext _context;

    public UsersHandler(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<long> AddUser(AddUserRequest request, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = new UserEntity
        {
            Name = request.Name
        };

        await _context.Users.AddAsync(user, token);
        await _context.SaveChangesAsync(token);

        return user.Id;
    }
}