using Microsoft.AspNetCore.Identity;

namespace Gartenzwerge.Infrastructure.Identity;

/// <summary>
/// This class represents an application user in the identity system. It extends the IdentityUser class provided by ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}