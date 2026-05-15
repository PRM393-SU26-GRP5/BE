namespace CourtManager.APIs.Attributes;

/// <summary>
/// Attribute to check if user has required role(s).
/// Usage: [RequireRole("Admin", "Manager")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireRoleAttribute : Attribute
{
    public string[] RequiredRoles { get; }

    public RequireRoleAttribute(params string[] roles)
    {
        RequiredRoles = roles;
    }
}
