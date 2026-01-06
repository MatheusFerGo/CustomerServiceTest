// Em: tests/ControlePedidos.CustomerContext.Tests/Application/Handlers/DeleteCustomerCommandHandlerTests.cs
using Moq;
using FluentAssertions;
using ControlePedidos.CustomerContext.Application.Commands.Delete;

namespace ControlePedidos.CustomerContext.Tests.Application.Handlers;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _handler = new DeleteCustomerCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenCustomerExists()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new DeleteCustomerCommand(customerId);

        _mockRepository.Setup(r => r.DeleteByIdAsync(customerId))
                       .ReturnsAsync(true);

        // ACT
        var result = await _handler.HandleAsync(command);

        // ASSERT
        // Verifica se o repositório foi chamado
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteByIdAsync(customerId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
    {
        // ARRANGE
        var customerId = Guid.NewGuid();
        var command = new DeleteCustomerCommand(customerId);

        _mockRepository.Setup(r => r.DeleteByIdAsync(customerId))
                       .ReturnsAsync(false);

        // ACT
        var result = await _handler.HandleAsync(command);

        // ASSERT
        // Verifica se o resultado do handler é 'false'
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteByIdAsync(customerId), Times.Once);
    }
}