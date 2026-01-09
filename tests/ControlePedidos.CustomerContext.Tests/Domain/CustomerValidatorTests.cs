using ControlePedidos.CustomerContext.Application.Validators;
using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Domain.Validations;
using FluentValidation.TestHelper;

namespace ControlePedidos.CustomerContext.Tests.Domain.Validators;

public class CustomerValidatorTests
{
    private readonly CustomerValidator _validator;

    public CustomerValidatorTests()
    {
        _validator = new CustomerValidator();
    }

    [Fact]
    public void Nao_Deve_Ter_Erro_Quando_Nome_For_Nulo()
    {
        // ARRANGE
        var model = new Customer
        {
            Name = null,
            Cpf = "12345678901"
        };

        // ACT
        var result = _validator.TestValidate(model);

        // ASSERT
        result.ShouldNotHaveValidationErrorFor(customer => customer.Name);
    }

    [Fact]
    public void Deve_Ter_Erro_Quando_Email_For_Invalido()
    {
        var model = new Customer { Email = "email-invalido" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(customer => customer.Email);
    }

    [Theory]
    [InlineData("123")] // Curto
    [InlineData("123456789012")] // Longo
    public void Deve_Ter_Erro_Quando_Cpf_For_Invalido(string cpf)
    {
        var model = new Customer { Cpf = cpf };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(customer => customer.Cpf);
    }

    [Fact]
    public void Nao_Deve_Ter_Erro_Quando_Cliente_For_Valido()
    {
        var model = new Customer
        {
            Name = "Matheus Fernandes",
            Email = "matheus@teste.com",
            Cpf = "12345678901"
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}