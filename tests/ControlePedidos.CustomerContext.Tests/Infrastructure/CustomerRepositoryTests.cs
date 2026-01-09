using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Infrastructure.Config;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace ControlePedidos.CustomerContext.Tests.Infrastructure;

public class CustomerRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    private CustomerDbContext _context;
    private CustomerRepository _repository;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        _context = new CustomerDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new CustomerRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_DevePermitirMultiplosClientesSemCpf()
    {
        // ARRANGE
        var customer1 = new Customer { Id = Guid.NewGuid(), Name = "Anon 1", Cpf = null };
        var customer2 = new Customer { Id = Guid.NewGuid(), Name = "Anon 2", Cpf = null };

        // ACT & ASSERT
        await _repository.Invoking(r => r.CreateAsync(customer1)).Should().NotThrowAsync();
        await _repository.Invoking(r => r.CreateAsync(customer2)).Should().NotThrowAsync();

        var all = await _repository.GetAllAsync();
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_DeveLancarErro_QuandoCpfForDuplicado()
    {
        // ARRANGE
        var cpf = "12345678901";
        var customer1 = new Customer { Id = Guid.NewGuid(), Cpf = cpf };
        var customer2 = new Customer { Id = Guid.NewGuid(), Cpf = cpf };

        // ACT
        await _repository.CreateAsync(customer1);

        // ASSERT
        await _repository.Invoking(r => r.CreateAsync(customer2))
            .Should().ThrowAsync<DbUpdateException>();
    }

    public async Task DisposeAsync() => await _postgresContainer.StopAsync();
}