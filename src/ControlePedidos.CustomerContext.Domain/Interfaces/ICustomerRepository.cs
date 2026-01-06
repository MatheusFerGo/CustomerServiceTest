using ControlePedidos.CustomerContext.Domain.Entities;

public interface ICustomerRepository
{
    Task CreateAsync(Customer customer);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByCpfAsync(string cpf);
    Task<Customer?> GetByEmailAsync(string email);
    Task UpdateAsync(Customer customer);
    Task<bool> DeleteByIdAsync(Guid id);
    Task<List<Customer>> GetListByIdsAsync(List<Guid> ids);
}