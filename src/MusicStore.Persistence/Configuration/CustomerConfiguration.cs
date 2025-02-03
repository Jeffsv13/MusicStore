using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Persistence.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsUnicode(false);

        builder.Property(x => x.FullName)
            .HasMaxLength(200);

        builder.ToTable(nameof(Customer), schema: "Musicales");
    }
}
