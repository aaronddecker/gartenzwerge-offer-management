namespace Gartenzwerge.Application.Common.Authorization;

/// <summary>
/// This class defines the application roles used for authorization purposes. It contains constant string values representing different roles that can be assigned to users within the application.
/// </summary>
public static class ApplicationRoles
{
    public const string Admin = "Admin";
    public const string Employee = "Employee";

    public const string AdminOrEmployee = Admin + "," + Employee;
}