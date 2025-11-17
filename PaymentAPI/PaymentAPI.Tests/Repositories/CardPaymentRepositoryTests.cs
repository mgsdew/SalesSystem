using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentAPI.Data;
using PaymentAPI.Models.Entities;
using PaymentAPI.Repositories;
using Xunit;

namespace PaymentAPI.Tests.Repositories;

public class CardPaymentRepositoryTests : IDisposable
{
    private readonly PaymentDbContext _context;
    private readonly Mock<ILogger<CardPaymentRepository>> _mockLogger;
    private readonly CardPaymentRepository _repository;

    public CardPaymentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDbContext(options);
        _mockLogger = new Mock<ILogger<CardPaymentRepository>>();
        _repository = new CardPaymentRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SaveAsync_Should_SaveCardPaymentAndReturnIt()
    {
        // Arrange
        var cardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.SaveAsync(cardPayment);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cardPayment.Id);
        result.CardNumber.Should().Be(cardPayment.CardNumber);
        result.IsValid.Should().Be(cardPayment.IsValid);
        result.CardType.Should().Be(cardPayment.CardType);
    }

    [Fact]
    public async Task SaveAsync_Should_ThrowArgumentNullException_WhenCardPaymentIsNull()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(null));
#pragma warning restore CS8625
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnCardPayment_WhenItExists()
    {
        // Arrange
        var cardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow
        };

        await _repository.SaveAsync(cardPayment);

        // Act
        var result = await _repository.GetByIdAsync(cardPayment.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cardPayment.Id);
        result.CardNumber.Should().Be(cardPayment.CardNumber);
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_WhenCardPaymentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAllCardPayments()
    {
        // Arrange
        var cardPayment1 = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow
        };

        var cardPayment2 = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "5425233430109903",
            IsValid = true,
            CardType = CardType.MasterCard,
            ValidatedAt = DateTime.UtcNow.AddMinutes(-1)
        };

        await _repository.SaveAsync(cardPayment1);
        await _repository.SaveAsync(cardPayment2);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().Contain(cp => cp.Id == cardPayment1.Id);
        result.Should().Contain(cp => cp.Id == cardPayment2.Id);
    }


    [Fact]
    public async Task DeleteAsync_Should_ReturnTrue_WhenCardPaymentExists()
    {
        // Arrange
        var cardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow
        };

        await _repository.SaveAsync(cardPayment);

        // Act
        var result = await _repository.DeleteAsync(cardPayment.Id);

        // Assert
        result.Should().BeTrue();

        // Verify it's actually deleted
        var deletedCardPayment = await _repository.GetByIdAsync(cardPayment.Id);
        deletedCardPayment.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnFalse_WhenCardPaymentDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SaveAsync_Should_UpdateExistingCardPayment()
    {
        // Arrange
        var cardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow
        };

        await _repository.SaveAsync(cardPayment);

        // Update the card payment
        cardPayment.IsValid = false;
        cardPayment.CardType = CardType.Unknown;

        // Act
        var result = await _repository.SaveAsync(cardPayment);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.CardType.Should().Be(CardType.Unknown);

        // Verify the update
        var updatedCardPayment = await _repository.GetByIdAsync(cardPayment.Id);
        updatedCardPayment.IsValid.Should().BeFalse();
        updatedCardPayment.CardType.Should().Be(CardType.Unknown);
    }

    [Fact]
    public async Task Repository_Should_HandleConcurrentOperations()
    {
        // Arrange
        var tasks = new List<Task>();
        var cardPayments = new List<CardPayment>();

        for (int i = 0; i < 10; i++)
        {
            var cardPayment = new CardPayment
            {
                Id = Guid.NewGuid().ToString(),
                CardNumber = $"453201511283036{i}",
                IsValid = true,
                CardType = CardType.Visa,
                ValidatedAt = DateTime.UtcNow
            };
            cardPayments.Add(cardPayment);
        }

        // Act - Save all concurrently
        foreach (var cardPayment in cardPayments)
        {
            tasks.Add(_repository.SaveAsync(cardPayment));
        }

        await Task.WhenAll(tasks);

        // Assert
        var allCardPayments = await _repository.GetAllAsync();
#pragma warning disable CS8602 // Dereference of a possibly null reference
        allCardPayments.Should().HaveCountGreaterThanOrEqualTo(10);
#pragma warning restore CS8602

        foreach (var cardPayment in cardPayments)
        {
            var found = await _repository.GetByIdAsync(cardPayment.Id);
            found.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnCardPaymentsInDescendingOrderByValidatedAt()
    {
        // Arrange
        var oldCardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "4532015112830366",
            IsValid = true,
            CardType = CardType.Visa,
            ValidatedAt = DateTime.UtcNow.AddHours(-2)
        };

        var newCardPayment = new CardPayment
        {
            Id = Guid.NewGuid().ToString(),
            CardNumber = "5425233430109903",
            IsValid = true,
            CardType = CardType.MasterCard,
            ValidatedAt = DateTime.UtcNow
        };

        await _repository.SaveAsync(oldCardPayment);
        await _repository.SaveAsync(newCardPayment);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        var resultList = result.ToList();
        resultList.Should().HaveCountGreaterThanOrEqualTo(2);
        
        // Verify descending order
        for (int i = 0; i < resultList.Count - 1; i++)
        {
            resultList[i].ValidatedAt.Should().BeOnOrAfter(resultList[i + 1].ValidatedAt);
        }
    }
}
