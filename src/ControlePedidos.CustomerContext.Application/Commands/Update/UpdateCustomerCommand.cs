namespace ControlePedidos.CustomerContext.Application.Commands.Update;

public class UpdateCustomerCommand
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public UpdateCustomerCommand(Guid id, string? name, string? email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}