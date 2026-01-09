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

    [Fact]
    public async Task CreateCustomer_DeveRetornar400BadRequest_QuandoCpfForInvalido()
    {
        // ARRANGE
        var command = new { cpf = "123", name = "Teste Erro", email = "erro@test.com" };

        // ACT
        var response = await _client.PostAsJsonAsync("/api/Customers/register", command);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("errors");
    }

    [Fact]
    public async Task CreateCustomer_DeveRetornar409Conflict_QuandoEmailJaExistir()
    {
        // ARRANGE
        var emailDuplicado = "duplicado@test.com";
        var command1 = new { cpf = "11122233344", name = "Cliente 1", email = emailDuplicado };
        await _client.PostAsJsonAsync("/api/Customers/register", command1);

        var command2 = new { cpf = "99988877766", name = "Cliente 2", email = emailDuplicado };

        // ACT
        var response = await _client.PostAsJsonAsync("/api/Customers/register", command2);

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("já cadastrado");
    }

    [Fact]
    public async Task GetByCpf_DeveRetornar404NotFound_QuandoCpfNaoExistir()
    {
        // ACT
        var response = await _client.GetAsync("/api/Customers/cpf/00000000000");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}