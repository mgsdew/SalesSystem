using UserAPI.Data;
using UserAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UserAPI.Tests.Repositories;

/// <summary>
/// Unit tests for UserRepository.
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly UserDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _userRepository = new UserRepository(_context, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Users.AddRange(new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@example.com",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = "cGFzc3dvcmQxMjNzYWx0",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "johndoe",
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = "cGFzc3dvcmQxMjNzYWx0",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        });
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Act
        var users = await _userRepository.GetAllUsersAsync();

        // Assert
        var userList = users.ToList();
        Assert.Equal(2, userList.Count); // Should have the seeded users
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange - Get the first user from the seeded data
        var allUsers = await _userRepository.GetAllUsersAsync();
        var firstUser = allUsers.First();

        // Act
        var user = await _userRepository.GetUserByIdAsync(firstUser.Id);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(firstUser.Id, user!.Id);
        Assert.Equal(firstUser.Username, user.Username);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Act
        var user = await _userRepository.GetUserByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsUser_WhenUsernameExists()
    {
        // Act
        var user = await _userRepository.GetUserByUsernameAsync("admin");

        // Assert
        Assert.NotNull(user);
        Assert.Equal("admin", user!.Username);
        Assert.Equal("admin@example.com", user.Email);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ReturnsNull_WhenUsernameDoesNotExist()
    {
        // Act
        var user = await _userRepository.GetUserByUsernameAsync("nonexistent");

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task AddUserAsync_AddsUserSuccessfully()
    {
        // Arrange
        var newUser = new User
        {
            Username = "newuser",
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User",
            PasswordHash = "hashedpassword"
        };

        // Act
        var addedUser = await _userRepository.AddUserAsync(newUser);

        // Assert
        Assert.NotNull(addedUser);
        Assert.NotEqual(Guid.Empty, addedUser.Id);
        Assert.Equal("newuser", addedUser.Username);
        Assert.Equal("newuser@example.com", addedUser.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var allUsers = await _userRepository.GetAllUsersAsync();
        var existingUser = allUsers.First();
        existingUser.FirstName = "Updated";

        // Act
        var result = await _userRepository.UpdateUserAsync(existingUser);

        // Assert
        Assert.True(result);
        var updatedUser = await _userRepository.GetUserByIdAsync(existingUser.Id);
        Assert.Equal("Updated", updatedUser!.FirstName);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUser = new User { Id = Guid.NewGuid(), Username = "test", Email = "test@example.com" };

        // Act
        var result = await _userRepository.UpdateUserAsync(nonExistentUser);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserByUsernameAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var newUser = new User
        {
            Username = "todelete",
            Email = "todelete@example.com",
            FirstName = "To",
            LastName = "Delete"
        };
        var addedUser = await _userRepository.AddUserAsync(newUser);

        // Act
        var result = await _userRepository.DeleteUserByUsernameAsync("todelete");

        // Assert
        Assert.True(result);
        var deletedUser = await _userRepository.GetUserByUsernameAsync("todelete");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUserByUsernameAsync_ReturnsFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _userRepository.DeleteUserByUsernameAsync("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UsernameExistsAsync_ReturnsTrue_WhenUsernameExists()
    {
        // Act
        var result = await _userRepository.UsernameExistsAsync("admin");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UsernameExistsAsync_ReturnsFalse_WhenUsernameDoesNotExist()
    {
        // Act
        var result = await _userRepository.UsernameExistsAsync("nonexistent");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsTrue_WhenEmailExists()
    {
        // Act
        var result = await _userRepository.EmailExistsAsync("admin@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_ReturnsFalse_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _userRepository.EmailExistsAsync("nonexistent@example.com");

        // Assert
        Assert.False(result);
    }
}