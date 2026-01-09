using System.Net;
using System.Net.Http.Json;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentAssertions;

namespace ControlePedidos.CustomerContext.Tests.Integration.Controllers;

public class CustomersControllerTests : IClassFixture<IntegrationTestBase>
{
    private readonly HttpClient _client;

    public CustomersControllerTests(IntegrationTestBase factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_DeveAceitarCpfVazio_ESalvarComoNull()
    {
        // ARRANGE
        var command = new { cpf = "", name = "Matheus", email = "matheus@test.com" }; //

        // ACT
        var response = await _client.PostAsJsonAsync("/api/Customers/register", command);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<Customer>();
        result.Cpf.Should().BeNull();
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoClienteNaoExistir()
    {
        // ACT
        var response = await _client.GetAsync($"/api/Customers/{Guid.NewGuid()}");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByCpf_DeveRetornar200_QuandoClienteExistir()
    {
        // ARRANGE
        var command = new { cpf = "12345678901", name = "Matheus", email = "m@test.com" };
        await _client.PostAsJsonAsync("/api/Customers/register", command);

        // ACT
        var response = await _client.GetAsync($"/api/Customers/cpf/12345678901");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<Customer>();
        customer!.Cpf.Should().Be("12345678901");
    }

    [Fact]
    public async Task GetAll_DeveRetornarListaDeClientes()
    {
        // ACT
        var response = await _client.GetAsync("/api/Customers");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
        customers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBatch_DeveRetornar413_QuandoExceder100Itens()
    {
        // ARRANGE
        var items = Enumerable.Range(0, 101)
            .Select(i => new { customerId = Guid.NewGuid() })
            .ToList();

        var request = new { items = items };

        // ACT
        var response = await _client.PostAsJsonAsync("/api/Customers/batch", request);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.RequestEntityTooLarge);
    }

    [Fact]
    public async Task Put_Update_DeveNormalizarCpfParaNull_QuandoCpfForVazio()
    {
        // 1. ARRANGE
        var createCommand = new { cpf = "12345678901", name = "Matheus Original", email = "m@original.com" };
        var createResponse = await _client.PostAsJsonAsync("/api/Customers/register", createCommand);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<Customer>();

        // 2. ACT
        var updateRequest = new
        {
            name = "Matheus Fernandes",
            email = "matheus@novo.com",
            cpf = ""
        };

        var response = await _client.PutAsJsonAsync($"/api/Customers/{createdCustomer!.Id}", updateRequest);

        // 3. ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/Customers/{createdCustomer.Id}");
        var updatedCustomer = await getResponse.Content.ReadFromJsonAsync<Customer>();

        updatedCustomer!.Cpf.Should().BeNull();
        updatedCustomer.Name.Should().Be("Matheus Fernandes");
    }
}