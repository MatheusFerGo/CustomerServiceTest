using ControlePedidos.CustomerContext.Application.Commands.Update;
using ControlePedidos.CustomerContext.Application.Handlers;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ControlePedidos.CustomerContext.Tests.Application.Handlers;

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IValidator<Customer>> _mockValidator;
    private readonly UpdateCustomerCommandHandler _handler;

    public UpdateCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockValidator = new Mock<IValidator<Customer>>();
        _handler = new UpdateCustomerCommandHandler(_mockValidator.Object, _mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarTrue_QuandoUpdateForBemSucedido()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(customerId, "Novo Nome", "novo@email.com", "12345678901");
        var existingCustomer = new Customer { Id = customerId, Name = "Antigo", Email = "antigo@email.com", Cpf = "00000000000" };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);
        _mockRepository.Setup(r => r.GetByCpfAsync(command.Cpf!)).ReturnsAsync((Customer?)null);
        _mockRepository.Setup(r => r.GetByEmailAsync(command.Email!)).ReturnsAsync((Customer?)null);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Customer>(), default)).ReturnsAsync(new ValidationResult());

        // ACT
        var result = await _handler.HandleAsync(command);

        // ASSERT
        result.Should().BeTrue();
        existingCustomer.Cpf.Should().Be("12345678901");
        _mockRepository.Verify(r => r.UpdateAsync(existingCustomer), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarFalse_QuandoClienteNaoExistir()
    {
        // ARRANGE
        var command = new UpdateCustomerCommand(Guid.NewGuid(), "Nome", "email@email.com", null);
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Customer?)null);

        // ACT
        var result = await _handler.HandleAsync(command);

        // ASSERT
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_DeveLancarExcecao_QuandoEmailJaEstiverEmUso()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(customerId, "Nome", "jaexiste@email.com", null);
        var currentCustomer = new Customer { Id = customerId, Email = "original@email.com" };
        var otherCustomer = new Customer { Id = Guid.NewGuid(), Email = "jaexiste@email.com" };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(currentCustomer);
        _mockRepository.Setup(r => r.GetByEmailAsync(command.Email!)).ReturnsAsync(otherCustomer);

        // ACT
        Func<Task> act = async () => await _handler.HandleAsync(command);

        // ASSERT
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("Este email já está sendo utilizado por outro cliente.");
    }

    [Fact]
    public async Task HandleAsync_DeveLancarExcecao_QuandoCpfJaEstiverEmUso()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(customerId, "Nome", "email@test.com", "99988877766");
        var currentCustomer = new Customer { Id = customerId, Cpf = null };
        var otherCustomer = new Customer { Id = Guid.NewGuid(), Cpf = "99988877766" };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(currentCustomer);
        _mockRepository.Setup(r => r.GetByCpfAsync(command.Cpf!)).ReturnsAsync(otherCustomer);

        // ACT & ASSERT
        await _handler.Invoking(h => h.HandleAsync(command))
                      .Should().ThrowAsync<Exception>()
                      .WithMessage("Este CPF já está sendo utilizado por outro cliente.");
    }

    [Fact]
    public async Task HandleAsync_NaoDeveValidarUnicidade_QuandoCpfForNulo()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand(customerId, "Anônimo", null, null);
        var existingCustomer = new Customer { Id = customerId, Cpf = null };

        _mockRepository.Setup(r => r.GetByIdAsync(customerId)).ReturnsAsync(existingCustomer);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Customer>(), default)).ReturnsAsync(new ValidationResult());

        // ACT
        await _handler.HandleAsync(command);

        // ASSERT
        _mockRepository.Verify(r => r.GetByCpfAsync(It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
    }
}