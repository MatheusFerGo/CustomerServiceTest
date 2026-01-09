using ControlePedidos.CustomerContext.Application.DTOs;
using ControlePedidos.CustomerContext.Application.Queries.GetBatch;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ControlePedidos.CustomerContext.Tests.Application.Queries;

public class GetCustomersBatchQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly GetCustomersBatchQueryHandler _handler;

    public GetCustomersBatchQueryHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _handler = new GetCustomersBatchQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_DeveRetornarSucesso_QuandoTodosOsClientesExistirem()
    {
        // ARRANGE
        var id1 = Guid.NewGuid();
        var request = new GetCustomerBatchRequest
        {
            Items = new List<BatchItemRequest> { new() { CustomerId = id1 } }
        };

        var customers = new List<Customer>
        {
            new() { Id = id1, Name = "Matheus", Email = "m@test.com", Cpf = "123" }
        };

        _mockRepository.Setup(r => r.GetListByIdsAsync(It.IsAny<List<Guid>>()))
                       .ReturnsAsync(customers);

        // ACT
        var result = await _handler.HandleAsync(request);

        // ASSERT
        result.Should().HaveCount(1);
        result.First().Customer.Name.Should().Be("Matheus");
        _mockRepository.Verify(r => r.GetListByIdsAsync(It.Is<List<Guid>>(l => l.Contains(id1))), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeveLancarInvalidOperation_QuandoExceder100Itens()
    {
        // ARRANGE
        var request = new GetCustomerBatchRequest();
        for (int i = 0; i < 101; i++)
            request.Items.Add(new BatchItemRequest { CustomerId = Guid.NewGuid() });

        // ACT
        Func<Task> act = async () => await _handler.HandleAsync(request);

        // ASSERT
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("PAYLOAD_TOO_LARGE");
    }

    [Fact]
    public async Task HandleAsync_DeveLancarArgumentException_QuandoListaForVazia()
    {
        // ARRANGE
        var request = new GetCustomerBatchRequest { Items = new List<BatchItemRequest>() };

        // ACT & ASSERT
        await _handler.Invoking(h => h.HandleAsync(request))
                      .Should().ThrowAsync<ArgumentException>()
                      .WithMessage("Bad Request: Items array is required and must have at least 1 element.");
    }

    [Fact]
    public async Task HandleAsync_DeveLancarKeyNotFound_QuandoRepositorioRetornarMenosItens()
    {
        // ARRANGE
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var request = new GetCustomerBatchRequest
        {
            Items = new List<BatchItemRequest> { new() { CustomerId = id1 }, new() { CustomerId = id2 } }
        };

        var customers = new List<Customer> { new() { Id = id1 } };
        _mockRepository.Setup(r => r.GetListByIdsAsync(It.IsAny<List<Guid>>()))
                       .ReturnsAsync(customers);

        // ACT & ASSERT
        await _handler.Invoking(h => h.HandleAsync(request))
                      .Should().ThrowAsync<KeyNotFoundException>();
    }
}