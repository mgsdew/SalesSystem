namespace UserAPI.Models.Entities;

/// <summary>
/// Represents the role of a user in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Regular user with basic permissions.
    /// </summary>
    User = 0,

    /// <summary>
    /// Administrator with elevated permissions.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Moderator with content management permissions.
    /// </summary>
    Moderator = 2
}