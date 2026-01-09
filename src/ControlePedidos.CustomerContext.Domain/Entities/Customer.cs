namespace ControlePedidos.CustomerContext.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Cpf { get; set; }

        public Customer() { }
    }
}