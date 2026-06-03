using Gartenzwerge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gartenzwerge.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the application.
///
/// The context exposes DbSets for all aggregate roots and applies
/// entity configurations from the Infrastructure assembly.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<OfferedService> OfferedServices => Set<OfferedService>();

    public DbSet<Offer> Offers => Set<Offer>();

    public DbSet<OfferItem> OfferItems => Set<OfferItem>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}