namespace UserAPI.Tests.Services;

/// <summary>
/// Unit tests for UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsMappedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "User" }
        };
        _userRepositoryMock.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        var userList = result.ToList();
        Assert.Single(userList);
        Assert.Equal("testuser", userList[0].Username);
        Assert.Equal("test@example.com", userList[0].Email);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsMappedUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "User" };
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByUsernameAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result!.Username);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("nonexistent")).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentException_WhenUsernameIsEmpty()
    {
        // Arrange
        var userDto = new UserRequestDto { Username = "", Email = "test@example.com", FirstName = "Test", LastName = "User" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentException_WhenEmailIsEmpty()
    {
        // Arrange
        var userDto = new UserRequestDto { Username = "testuser", Email = "", FirstName = "Test", LastName = "User" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentException_WhenFirstNameIsEmpty()
    {
        // Arrange
        var userDto = new UserRequestDto { Username = "testuser", Email = "test@example.com", FirstName = "", LastName = "User" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsArgumentException_WhenLastNameIsEmpty()
    {
        // Arrange
        var userDto = new UserRequestDto { Username = "testuser", Email = "test@example.com", FirstName = "Test", LastName = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsInvalidOperationException_WhenUsernameExists()
    {
        // Arrange
        var userDto = new UserRequestDto
        {
            Username = "existinguser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync("existinguser")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_ThrowsInvalidOperationException_WhenEmailExists()
    {
        // Arrange
        var userDto = new UserRequestDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync("newuser")).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.EmailExistsAsync("existing@example.com")).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(userDto));
    }

    [Fact]
    public async Task CreateUserAsync_CreatesUser_WhenValidData()
    {
        // Arrange
        var userDto = new UserRequestDto
        {
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "password123"
        };
        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "newuser",
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User"
        };
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync("newuser")).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.EmailExistsAsync("new@example.com")).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(userDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.Username);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userDto = new UserRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User"
        };
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("nonexistent")).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync("nonexistent", userDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserByUsernameAsync("testuser")).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync("testuser");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserByUsernameAsync("nonexistent")).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsUser_WhenValidCredentials()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "cGFzc3dvcmQxMjNzYWx0", // base64 encoded "password123salt"
            IsActive = true
        };
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateUserAsync("testuser", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result!.Username);
    }

    [Fact]
    public async Task AuthenticateUserAsync_ReturnsNull_WhenInvalidCredentials()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "cGFzc3dvcmQxMjNzYWx0", // base64 encoded "password123salt"
            IsActive = true
        };
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _userService.AuthenticateUserAsync("testuser", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AssignRoleAsync_ReturnsUpdatedUser_WhenAdminAssignsRole()
    {
        // Arrange
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };
        var targetUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "targetuser",
            Email = "target@example.com",
            Role = null,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("admin")).ReturnsAsync(adminUser);
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("targetuser")).ReturnsAsync(targetUser);
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

        // Act
        var result = await _userService.AssignRoleAsync("targetuser", UserRole.User, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("targetuser", result!.Username);
        Assert.Equal(UserRole.User, result.Role);
    }

    [Fact]
    public async Task AssignRoleAsync_ThrowsUnauthorizedAccessException_WhenNonAdminTriesToAssignRole()
    {
        // Arrange
        var regularUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "regularuser",
            Email = "regular@example.com",
            Role = UserRole.User,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("regularuser")).ReturnsAsync(regularUser);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _userService.AssignRoleAsync("targetuser", UserRole.Admin, "regularuser"));
    }

    [Fact]
    public async Task AssignRoleAsync_ReturnsNull_WhenTargetUserDoesNotExist()
    {
        // Arrange
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@example.com",
            Role = UserRole.Admin,
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("admin")).ReturnsAsync(adminUser);
        _userRepositoryMock.Setup(x => x.GetUserByUsernameAsync("nonexistent")).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.AssignRoleAsync("nonexistent", UserRole.User, "admin");

        // Assert
        Assert.Null(result);
    }
}