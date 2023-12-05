using CurrencyConversion.Data.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CurrencyConversion.Data.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        return services;
    }
}