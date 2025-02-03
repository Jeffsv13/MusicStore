using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;

namespace MusicStore.Persistence.Configuration;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.Property(x => x.OperationNumber)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(x => x.SaleDate)
            .HasColumnType("date")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(x => x.Total)
            .HasColumnType("decimal(10,2)");

        builder.ToTable(nameof(Sale), schema: "Musicales");
    }
}
