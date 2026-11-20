using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCleanApi.Domain.Entities;

namespace MyCleanApi.Infrastructure.Persistence.Configurations;

public class SampleItemConfiguration : IEntityTypeConfiguration<SampleItem>
{
    public void Configure(EntityTypeBuilder<SampleItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);
    }
}
