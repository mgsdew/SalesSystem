using Microsoft.EntityFrameworkCore;
using UserAPI.Models.Entities;

namespace UserAPI.Data;

/// <summary>
/// Database context for UserAPI operations.
/// </summary>
public class UserDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("tblUser");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier")
                .IsRequired()
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Username)
                .HasColumnName("Username")
                .HasColumnType("varchar(50)")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Email)
                .HasColumnName("Email")
                .HasColumnType("varchar(255)")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FirstName)
                .HasColumnName("FirstName")
                .HasColumnType("varchar(100)")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .HasColumnName("LastName")
                .HasColumnType("varchar(100)")
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasColumnType("varchar(255)")
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Role)
                .HasColumnName("Role")
                .HasColumnType("int")
                .IsRequired(false)
                .HasConversion<int?>(); // Store enum as int, nullable

            entity.Property(e => e.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("datetime2")
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Create unique indexes
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("IX_User_Username");

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email");
        });
    }
}