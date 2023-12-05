using CurrencyConversion.Api.Contracts.Accounts.Requests;
using CurrencyConversion.Services.Accounts;
using CurrencyConversion.Services.Accounts.Validators;
using CurrencyConversion.Services.Currencies;
using CurrencyConversion.Services.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConversion.Services.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersHandler, UsersHandler>();
        services.AddScoped<ICurrenciesHandler, CurrenciesHandler>();
        services.AddScoped<IAccountsHandler, AccountsHandler>();

        services.AddScoped<IValidator<ConvertRequest>, ConvertRequestValidator>();
        services.AddScoped<IValidator<TopUpRequest>, TopUpRequestValidator>();

        return services;
    }
}