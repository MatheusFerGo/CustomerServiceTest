using ControlePedidos.CustomerContext.Application.DTOs;

namespace ControlePedidos.CustomerContext.Application.Queries.GetBatch
{
    public class GetCustomersBatchQueryHandler
    {
        private readonly ICustomerRepository _repository;

        public GetCustomersBatchQueryHandler(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomerBatchResponseItem>> HandleAsync(GetCustomerBatchRequest request)
        {
            // REGRA 1: Validação de Payload (Max 100)
            if (request.Items.Count > 100)
            {
                throw new InvalidOperationException("PAYLOAD_TOO_LARGE");
            }

            // REGRA 2: Pelo menos 1 item
            if (request.Items == null || !request.Items.Any())
            {
                throw new ArgumentException("Bad Request: Items array is required and must have at least 1 element.");
            }

            var idsToFind = request.Items.Select(x => x.CustomerId).Distinct().ToList();

            var customers = await _repository.GetListByIdsAsync(idsToFind);

            // REGRA 3: Tudo ou Nada (Fail Fast)
            if (customers.Count != idsToFind.Count)
            {
                throw new KeyNotFoundException("One or more customers not found within the provided batch.");
            }

            var response = customers.Select(c => new CustomerBatchResponseItem
            {
                Id = c.Id,
                Customer = new CustomerDetailDto
                {
                    Name = c.Name,
                    Email = c.Email,
                    Cpf = c.Cpf
                }
            }).ToList();

            return response;
        }
    }
}