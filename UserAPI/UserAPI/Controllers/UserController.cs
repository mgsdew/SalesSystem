using Microsoft.AspNetCore.Mvc;
using UserAPI.Models.DTOs;
using UserAPI.Services.Interfaces;

namespace UserAPI.Controllers;

/// <summary>
/// Controller for managing user operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            return StatusCode(500, new { error = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    /// <param name="id">The user GUID.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("by-id/{id}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the user" });
        }
    }

    /// <summary>
    /// Gets a specific user by username.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with username {Username}", username);
            return StatusCode(500, new { error = "An error occurred while retrieving the user" });
        }
    }

    /// <summary>
    /// Gets a user by email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">User not found.</response>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { error = "Email is required" });
            }

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            return StatusCode(500, new { error = "An error occurred while retrieving the user" });
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userDto">The user data.</param>
    /// <returns>The created user.</returns>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid user data.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] UserRequestDto userDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdUser = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserByUsername), new { username = createdUser.Username }, createdUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { error = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <param name="userDto">The updated user data.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">User updated successfully.</response>
    /// <response code="404">User not found.</response>
    /// <response code="400">Invalid user data.</response>
    [HttpPut("{username}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(string username, [FromBody] UserRequestDto userDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedUser = await _userService.UpdateUserAsync(username, userDto);
            if (updatedUser == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(updatedUser);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with username {Username}", username);
            return StatusCode(500, new { error = "An error occurred while updating the user" });
        }
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="username">The user's username.</param>
    /// <returns>No content.</returns>
    /// <response code="204">User deleted successfully.</response>
    /// <response code="404">User not found.</response>
    [HttpDelete("{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string username)
    {
        try
        {
            var success = await _userService.DeleteUserAsync(username);
            if (!success)
            {
                return NotFound(new { error = "User not found" });
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with username {Username}", username);
            return StatusCode(500, new { error = "An error occurred while deleting the user" });
        }
    }

    /// <summary>
    /// Authenticates a user.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>The authenticated user.</returns>
    /// <response code="200">Authentication successful.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate([FromBody] LoginRequestDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.AuthenticateUserAsync(loginDto.UsernameOrEmail, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { error = "Invalid username/email or password" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user with username/email {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return StatusCode(500, new { error = "An error occurred during authentication" });
        }
    }

    /// <summary>
    /// Assigns a role to a user. Only administrators can perform this action.
    /// </summary>
    /// <param name="username">The username of the user to assign the role to.</param>
    /// <param name="assignRoleDto">The role assignment data.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Role assigned successfully.</response>
    /// <response code="403">Forbidden - only admins can assign roles.</response>
    /// <response code="404">User not found.</response>
    /// <response code="400">Invalid role assignment data.</response>
    [HttpPost("{username}/assign-role")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole(string username, [FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // In a real implementation, get the requesting user from the authentication context
            // For now, we'll need to pass it as a header or query parameter
            // This is a simplified implementation for demo purposes
            var requestingUsername = HttpContext.Request.Headers["X-Requesting-User"].ToString();
            if (string.IsNullOrEmpty(requestingUsername))
            {
                return BadRequest(new { error = "X-Requesting-User header is required" });
            }

            var updatedUser = await _userService.AssignRoleAsync(username, assignRoleDto.Role, requestingUsername);
            if (updatedUser == null)
            {
                return NotFound(new { error = "User not found" });
            }
            return Ok(updatedUser);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user {Username}", username);
            return StatusCode(500, new { error = "An error occurred while assigning the role" });
        }
    }
}

/// <summary>
/// DTO for login requests.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// User's username or email address.
    /// </summary>
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}