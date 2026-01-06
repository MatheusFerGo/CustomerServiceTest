using ControlePedidos.CustomerContext.Application.Commands.Create;
using ControlePedidos.CustomerContext.Application.Commands.Delete;
using ControlePedidos.CustomerContext.Application.Commands.Update;
using ControlePedidos.CustomerContext.Application.DTOs;
using ControlePedidos.CustomerContext.Application.Handlers;
using ControlePedidos.CustomerContext.Application.Queries.GetBatch;
using ControlePedidos.CustomerContext.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ControlePedidos.CustomerContext.Api.Controllers;

    [ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerCommandHandler _createHandler;
    private readonly DeleteCustomerCommandHandler _deleteHandler;
    private readonly UpdateCustomerCommandHandler _updateHandler;
    private readonly GetCustomersBatchQueryHandler _batchHandler;
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(
        CreateCustomerCommandHandler createHandler,
        DeleteCustomerCommandHandler deleteHandler,
        UpdateCustomerCommandHandler updateHandler,
        GetCustomersBatchQueryHandler batchHandler,
        ICustomerRepository customerRepository)
    {
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
        _updateHandler = updateHandler;
        _batchHandler = batchHandler;
        _customerRepository = customerRepository;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        try
        {
            var newCustomer = await _createHandler.HandleAsync(command);
            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.Id }, newCustomer);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Customer>> GetCustomerById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound(new { message = "Cliente não encontrado." });
        }
        return Ok(customer);
    }

    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Customer>> GetCustomerByCpf(string cpf)
    {
        var customer = await _customerRepository.GetByCpfAsync(cpf);
        if (customer == null)
        {
            return NotFound(new { message = "Cliente não encontrado." });
        }
        return Ok(customer);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var command = new UpdateCustomerCommand(id, request.Name, request.Email);
        var result = await _updateHandler.HandleAsync(command);

        if (!result) return NotFound(new { message = "Cliente não encontrado." });

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var command = new DeleteCustomerCommand(id);
        var success = await _deleteHandler.HandleAsync(command);

        if (!success)
        {
            return NotFound(new { message = "Cliente não encontrado para exclusão." });
        }

        return NoContent();
    }

    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<CustomerBatchResponseItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBatch([FromBody] GetCustomerBatchRequest request)
    {
        try
        {
            var result = await _batchHandler.HandleAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message == "PAYLOAD_TOO_LARGE")
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge,
                new { mensagem = "O limite máximo de 100 itens por requisição foi excedido." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = "Um ou mais clientes informados no lote não foram encontrados." });
        }
        catch (FluentValidation.ValidationException ex)
        {
            // Evita o erro 500 da image_e226a8.png
            return BadRequest(new { erros = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { mensagem = "Ocorreu um erro inesperado ao processar o lote." });
        }
    }
}