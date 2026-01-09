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

        var cpfNormalizado = string.IsNullOrWhiteSpace(command.Cpf) ? null : command.Cpf;
        var emailNormalizado = string.IsNullOrWhiteSpace(command.Email) ? null : command.Email;
        var nameNormalizado = string.IsNullOrWhiteSpace(command.Name) ? null : command.Name;

        if (cpfNormalizado != null && cpfNormalizado != customer.Cpf)
        {
            var existingCpf = await _customerRepository.GetByCpfAsync(cpfNormalizado);
            if (existingCpf != null)
            {
                throw new Exception("Este CPF já está sendo utilizado por outro cliente.");
            }
        }

        if (emailNormalizado != null && emailNormalizado != customer.Email)
        {
            var existingEmail = await _customerRepository.GetByEmailAsync(emailNormalizado);
            if (existingEmail != null)
            {
                throw new Exception("Este email já está sendo utilizado por outro cliente.");
            }
        }

        customer.Name = nameNormalizado;
        customer.Email = emailNormalizado;
        customer.Cpf = cpfNormalizado;

        var validationResult = await _customerValidator.ValidateAsync(customer);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        await _customerRepository.UpdateAsync(customer);
        return true;
    }
}