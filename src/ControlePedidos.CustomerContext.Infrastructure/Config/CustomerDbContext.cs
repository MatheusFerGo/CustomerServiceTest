using ControlePedidos.CustomerContext.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ControlePedidos.CustomerContext.Infrastructure.Config;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}