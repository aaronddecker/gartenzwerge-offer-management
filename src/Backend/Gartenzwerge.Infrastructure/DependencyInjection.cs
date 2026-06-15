using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Application.Orders.Interfaces;
using Gartenzwerge.Application.Auth.Interfaces;
using Gartenzwerge.Infrastructure.Persistence;
using Gartenzwerge.Infrastructure.Repositories;
using Gartenzwerge.Infrastructure.Identity;
using Gartenzwerge.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
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
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}