using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserAPI.Tests.Controllers;

/// <summary>
/// Unit tests for UserController.
/// </summary>
public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkResult_WithUsers()
    {
        // Arrange
        var users = new List<UserResponseDto>
        {
            new UserResponseDto { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "User" }
        };
        _userServiceMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsType<List<UserResponseDto>>(okResult.Value);
        Assert.Single(returnedUsers);
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var user = new UserResponseDto { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "User" };
        _userServiceMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserByUsername("testuser");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task GetUserByUsername_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUserByUsernameAsync("nonexistent")).ReturnsAsync((UserResponseDto?)null);

        // Act
        var result = await _controller.GetUserByUsername("nonexistent");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedResult_WhenValidData()
    {
        // Arrange
        var userRequest = new UserRequestDto
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };
        var createdUser = new UserResponseDto
        {
            Id = Guid.NewGuid(),
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User"
        };
        _userServiceMock.Setup(x => x.CreateUserAsync(userRequest)).ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateUser(userRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedUser = Assert.IsType<UserResponseDto>(createdResult.Value);
        Assert.Equal("newuser", returnedUser.Username);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenInvalidData()
    {
        // Arrange
        var userRequest = new UserRequestDto(); // Empty request
        _controller.ModelState.AddModelError("Username", "Username is required");

        // Act
        var result = await _controller.CreateUser(userRequest);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var userRequest = new UserRequestDto
        {
            Username = "updateduser",
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User"
        };
        var updatedUser = new UserResponseDto
        {
            Id = Guid.NewGuid(),
            Username = "updateduser",
            Email = "updated@example.com",
            FirstName = "Updated",
            LastName = "User"
        };
        _userServiceMock.Setup(x => x.UpdateUserAsync("testuser", userRequest)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser("testuser", userRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
        Assert.Equal("updateduser", returnedUser.Username);
    }

    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userRequest = new UserRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        _userServiceMock.Setup(x => x.UpdateUserAsync("nonexistent", userRequest)).ReturnsAsync((UserResponseDto?)null);

        // Act
        var result = await _controller.UpdateUser("nonexistent", userRequest);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenUserExists()
    {
        // Arrange
        _userServiceMock.Setup(x => x.DeleteUserAsync("testuser")).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser("testuser");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userServiceMock.Setup(x => x.DeleteUserAsync("nonexistent")).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser("nonexistent");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AssignRole_ReturnsOkResult_WhenAdminAssignsRole()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto { Role = UserRole.User };
        var updatedUser = new UserResponseDto
        {
            Id = Guid.NewGuid(),
            Username = "targetuser",
            Email = "target@example.com",
            Role = UserRole.User
        };
        _userServiceMock.Setup(x => x.AssignRoleAsync("targetuser", UserRole.User, "admin")).ReturnsAsync(updatedUser);

        // Set up the request context to include the X-Requesting-User header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Requesting-User"] = "admin";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.AssignRole("targetuser", assignRoleDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<UserResponseDto>(okResult.Value);
        Assert.Equal(UserRole.User, returnedUser.Role);
    }

    [Fact]
    public async Task AssignRole_ReturnsBadRequest_WhenRequestingUserHeaderIsMissing()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto { Role = UserRole.User };

        // Set up the request context without the X-Requesting-User header
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.AssignRole("targetuser", assignRoleDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("X-Requesting-User header is required", badRequestResult.Value?.ToString());
    }

    [Fact]
    public async Task AssignRole_ReturnsForbid_WhenNonAdminTriesToAssignRole()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto { Role = UserRole.Admin };
        _userServiceMock.Setup(x => x.AssignRoleAsync("targetuser", UserRole.Admin, "regularuser"))
            .ThrowsAsync(new UnauthorizedAccessException("Only administrators can assign roles"));

        // Set up the request context to include the X-Requesting-User header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Requesting-User"] = "regularuser";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.AssignRole("targetuser", assignRoleDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task AssignRole_ReturnsNotFound_WhenTargetUserDoesNotExist()
    {
        // Arrange
        var assignRoleDto = new AssignRoleDto { Role = UserRole.User };
        _userServiceMock.Setup(x => x.AssignRoleAsync("nonexistent", UserRole.User, "admin")).ReturnsAsync((UserResponseDto?)null);

        // Set up the request context to include the X-Requesting-User header
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Requesting-User"] = "admin";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = await _controller.AssignRole("nonexistent", assignRoleDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}