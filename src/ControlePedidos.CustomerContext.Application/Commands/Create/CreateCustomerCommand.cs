namespace ControlePedidos.CustomerContext.Application.Commands.Create;

public record CreateCustomerCommand(
    string Cpf,
    string? Name,
    string? Email
);