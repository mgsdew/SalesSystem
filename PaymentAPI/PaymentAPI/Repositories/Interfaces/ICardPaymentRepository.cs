using PaymentAPI.Models.Entities;

namespace PaymentAPI.Repositories.Interfaces;

/// <summary>
/// Interface for card payment repository operations.
/// </summary>
public interface ICardPaymentRepository
{
    /// <summary>
    /// Saves a card payment validation record asynchronously.
    /// </summary>
    /// <param name="cardPayment">The card payment entity to save.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the saved card payment entity.</returns>
    Task<CardPayment> SaveAsync(CardPayment cardPayment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a card payment validation record by ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the card payment record.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the card payment entity if found; otherwise, null.</returns>
    Task<CardPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all card payment validation records asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of card payment entities.</returns>
    Task<IEnumerable<CardPayment>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a card payment validation record by ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the card payment record to delete.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
