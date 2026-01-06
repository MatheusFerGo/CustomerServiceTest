using ControlePedidos.CustomerContext.Application.Commands.Delete;
using ControlePedidos.CustomerContext.Application.Handlers;
using ControlePedidos.CustomerContext.Application.Queries.GetBatch;
using ControlePedidos.CustomerContext.Application.Validators;
using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Infrastructure;
using ControlePedidos.CustomerContext.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IValidator<Customer>, CustomerValidator>();

builder.Services.AddScoped<CreateCustomerCommandHandler>();
builder.Services.AddScoped<DeleteCustomerCommandHandler>();
builder.Services.AddScoped<UpdateCustomerCommandHandler>();
builder.Services.AddScoped<GetCustomersBatchQueryHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection(); // Deixe apenas para produção real
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();