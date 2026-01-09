using System.Text.Json.Serialization;

namespace ControlePedidos.CustomerContext.Application.DTOs
{
    // --- REQUEST ---

    public class BatchItemRequest
    {
        [JsonPropertyName("customerId")]
        public Guid CustomerId { get; set; }
    }

    public class GetCustomerBatchRequest
    {
        [JsonPropertyName("items")]
        public List<BatchItemRequest> Items { get; set; } = new();
    }

    // --- RESPONSE ---
    public class CustomerDetailDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
    }

    public class CustomerBatchResponseItem
    {
        public Guid Id { get; set; }
        
        public CustomerDetailDto Customer { get; set; } 
    }
}