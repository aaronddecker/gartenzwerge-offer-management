using Gartenzwerge.Application.Common.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gartenzwerge.Infrastructure.Identity;

/// <summary>
/// This class is responsible for seeding the initial roles and admin user into the identity system.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedRolesAndDevelopmentUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var roles = new[]
        {
            ApplicationRoles.Admin,
            ApplicationRoles.Employee
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var adminUser = await userManager.FindByEmailAsync("test@gartenzwerge.de");

        if (adminUser is not null &&
            !await userManager.IsInRoleAsync(adminUser, ApplicationRoles.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
        }

        var employeeUser = await userManager.FindByEmailAsync("employee@gartenzwerge.de");

        if (employeeUser is null)
        {
            employeeUser = new ApplicationUser
            {
                UserName = "employee@gartenzwerge.de",
                Email = "employee@gartenzwerge.de",
                DisplayName = "Employee User"
            };

            await userManager.CreateAsync(employeeUser, "Test1234");
        }

        if (!await userManager.IsInRoleAsync(employeeUser, ApplicationRoles.Employee))
        {
            await userManager.AddToRoleAsync(employeeUser, ApplicationRoles.Employee);
        }
    }
}