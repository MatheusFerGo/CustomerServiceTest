using ControlePedidos.CustomerContext.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControlePedidos.CustomerContext.Infrastructure.Persistence.Mapping;

public class CustomerMap : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.Cpf)
               .IsUnique();

        builder.Property(e => e.Name)
               .IsRequired(false)
               .HasMaxLength(150);

        builder.HasIndex(e => e.Email)
               .IsUnique();

        builder.Property(e => e.Email)
               .IsRequired(false)
               .HasMaxLength(150);
    }
}