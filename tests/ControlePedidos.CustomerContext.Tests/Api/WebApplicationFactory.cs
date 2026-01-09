using ControlePedidos.CustomerContext.Infrastructure.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.PostgreSql;

public class IntegrationTestBase : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext original e adiciona um apontando para o Testcontainer
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CustomerDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<CustomerDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        await context.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync() => await _dbContainer.StopAsync();
}