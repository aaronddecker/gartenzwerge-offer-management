using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Infrastructure.Persistence;
using Gartenzwerge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gartenzwerge.Infrastructure;

/// <summary>
/// Registers infrastructure-related services such as persistence and repositories.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOfferedServiceRepository, OfferedServiceRepository>();

        return services;
    }
}