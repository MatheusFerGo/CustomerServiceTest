using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Design;

using Microsoft.Extensions.Configuration;

using System.IO;



namespace ControlePedidos.CustomerContext.Infrastructure.Persistence;



public class CustomerDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>

{

    public CustomerDbContext CreateDbContext(string[] args)

    {

        IConfigurationRoot configuration = new ConfigurationBuilder()

            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ControlePedidos.CustomerContext.Api"))

            .AddJsonFile("appsettings.json")

            .AddJsonFile("appsettings.Development.json", optional: true)

            .Build();



        var connectionString = configuration.GetConnectionString("DefaultConnection");



        var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));



        return new CustomerDbContext(optionsBuilder.Options);

    }

}