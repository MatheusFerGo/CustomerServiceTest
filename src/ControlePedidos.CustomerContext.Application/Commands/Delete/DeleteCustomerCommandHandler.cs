using ControlePedidos.CustomerContext.Domain;

namespace ControlePedidos.CustomerContext.Application.Commands.Delete;

public class DeleteCustomerCommandHandler
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<bool> HandleAsync(DeleteCustomerCommand command)
    {
        // Pro futuro: ("não deletar cliente com pedidos"?)

        return await _customerRepository.DeleteByIdAsync(command.Id);
    }
}