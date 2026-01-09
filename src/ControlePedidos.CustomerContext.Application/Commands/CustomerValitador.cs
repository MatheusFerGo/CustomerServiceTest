using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Domain.Validations;
using FluentValidation;

namespace ControlePedidos.CustomerContext.Application.Validators;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(c => c.Cpf)
                    .Must(CpfValidator.IsValid)
                    .When(c => !string.IsNullOrWhiteSpace(c.Cpf))
                    .WithMessage("O CPF fornecido é inválido.");

        RuleFor(c => c.Name)
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("O email informado não é válido.")
            .When(c => !string.IsNullOrWhiteSpace(c.Email));
    }
}
