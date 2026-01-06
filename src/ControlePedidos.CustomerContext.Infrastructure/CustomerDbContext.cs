using ControlePedidos.CustomerContext.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlePedidos.CustomerContext.Infrastructure
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Cpf)
                      .IsUnique();

                entity.Property(e => e.Name)
                      .IsRequired(false);

                entity.HasIndex(e => e.Email)
                      .IsUnique();

                entity.Property(e => e.Email)
                      .IsRequired(false);
            });
        }
    }
}