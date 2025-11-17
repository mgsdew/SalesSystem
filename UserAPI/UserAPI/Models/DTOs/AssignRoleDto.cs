using UserAPI.Models.Entities;

namespace UserAPI.Models.DTOs;

/// <summary>
/// DTO for role assignment requests.
/// </summary>
public class AssignRoleDto
{
    /// <summary>
    /// Gets or sets the role to assign to the user.
    /// </summary>
    public UserRole Role { get; set; }
}