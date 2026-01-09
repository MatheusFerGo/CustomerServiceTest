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
        var cpfNormalizado = string.IsNullOrWhiteSpace(command.Cpf) ? null : command.Cpf;
        var emailNormalizado = string.IsNullOrWhiteSpace(command.Email) ? null : command.Email;

        if (cpfNormalizado != null)
        {
            var existing = await _customerRepository.GetByCpfAsync(cpfNormalizado);
            if (existing != null) throw new Exception("CPF já cadastrado.");
        }

        if (emailNormalizado != null)
        {
            var existingEmail = await _customerRepository.GetByEmailAsync(emailNormalizado);
            if (existingEmail != null) throw new Exception("E-mail já cadastrado.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Cpf = cpfNormalizado,
            Name = command.Name,
            Email = emailNormalizado
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