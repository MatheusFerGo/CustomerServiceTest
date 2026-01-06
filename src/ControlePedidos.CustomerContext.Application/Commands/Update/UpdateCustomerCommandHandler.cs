using ControlePedidos.CustomerContext.Application.Commands.Update;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentValidation;

namespace ControlePedidos.CustomerContext.Application.Handlers;

public class UpdateCustomerCommandHandler
{
    private readonly IValidator<Customer> _customerValidator;
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerCommandHandler(
        IValidator<Customer> customerValidator,
        ICustomerRepository customerRepository)
    {
        _customerValidator = customerValidator;
        _customerRepository = customerRepository;
    }

    public async Task<bool> HandleAsync(UpdateCustomerCommand command)
    {
        var customer = await _customerRepository.GetByIdAsync(command.Id);
        if (customer == null) return false;

        if (!string.IsNullOrWhiteSpace(command.Email) && command.Email != customer.Email)
        {
            var existingEmail = await _customerRepository.GetByEmailAsync(command.Email);
            if (existingEmail != null)
            {
                throw new Exception("Este email já está sendo utilizado por outro cliente.");
            }
        }

        customer.Name = command.Name;
        customer.Email = command.Email;

        var validationResult = await _customerValidator.ValidateAsync(customer);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _customerRepository.UpdateAsync(customer);
        return true;
    }
}