using Gartenzwerge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gartenzwerge.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Company)
            .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Street)
            .HasMaxLength(150);

        builder.Property(x => x.HouseNumber)
            .HasMaxLength(20);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(20);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}