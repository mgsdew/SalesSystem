using Microsoft.EntityFrameworkCore;
using PaymentAPI.Models.Entities;

namespace PaymentAPI.Data;

public class PaymentDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the card payments DbSet.
    /// </summary>
    public DbSet<CardPayment> CardPayments { get; set; }

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure CardPayment entity
        modelBuilder.Entity<CardPayment>(entity =>
        {
            entity.ToTable("tblCardPayment");

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .HasColumnType("varchar(50)")
                .IsRequired();

            entity.Property(e => e.CardNumber)
                .HasColumnName("CardNumber")
                .HasColumnType("varchar(20)")
                .IsRequired();

            entity.Property(e => e.IsValid)
                .HasColumnName("IsValid")
                .HasColumnType("bit")
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.ValidatedAt)
                .HasColumnName("ValidatedAt")
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.CardType)
                .HasColumnName("CardType")
                .HasColumnType("varchar(10)")
                .IsRequired()
                .HasConversion<string>(); // Store enum as string
        });
    }
}