using Gartenzwerge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gartenzwerge.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for offered services.
/// </summary>
public class OfferedServiceConfiguration : IEntityTypeConfiguration<OfferedService>
{
    public void Configure(EntityTypeBuilder<OfferedService> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Unit)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.BasePrice)
            .HasPrecision(18, 2);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}