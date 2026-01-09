using ControlePedidos.CustomerContext.Domain.Entities;
using ControlePedidos.CustomerContext.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _context;

    public CustomerRepository(CustomerDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.AsNoTracking().ToListAsync();
    }

    public async Task<Customer?> GetByCpfAsync(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return null;

        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Cpf == cpf);
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<Customer?> GetByEmailAsync(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Customer>> GetListByIdsAsync(List<Guid> ids)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .ToListAsync();
    }
}