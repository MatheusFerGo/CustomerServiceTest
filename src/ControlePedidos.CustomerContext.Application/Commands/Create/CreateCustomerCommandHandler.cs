using ControlePedidos.CustomerContext.Application.Commands.Create;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentValidation;

namespace ControlePedidos.CustomerContext.Application.Handlers;

public class CreateCustomerCommandHandler
{
    private readonly IValidator<Customer> _customerValidator;
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(
        IValidator<Customer> customerValidator,
        ICustomerRepository customerRepository)
    {
        _customerValidator = customerValidator;
        _customerRepository = customerRepository;
    }

    public async Task<Customer> HandleAsync(CreateCustomerCommand command)
    {
        var existingCustomer = await _customerRepository.GetByCpfAsync(command.Cpf);
        if (existingCustomer != null)
        {
            throw new Exception("Cliente com este CPF já cadastrado.");
        }

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var existingEmail = await _customerRepository.GetByEmailAsync(command.Email);
            if (existingEmail != null)
            {
                throw new Exception("Cliente com este Email já cadastrado.");
            }
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Cpf = command.Cpf,
            Name = command.Name,
            Email = command.Email
        };

        var validationResult = await _customerValidator.ValidateAsync(customer);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _customerRepository.CreateAsync(customer);

        return customer;
    }
}