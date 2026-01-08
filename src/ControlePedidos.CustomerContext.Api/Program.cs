using ControlePedidos.CustomerContext.Application.Commands.Delete;
using ControlePedidos.CustomerContext.Application.Handlers;
using ControlePedidos.CustomerContext.Application.Queries.GetBatch;
using ControlePedidos.CustomerContext.Application.Validators;
using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Infrastructure.Config;
using ControlePedidos.CustomerContext.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseNpgsql(connectionString));

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();