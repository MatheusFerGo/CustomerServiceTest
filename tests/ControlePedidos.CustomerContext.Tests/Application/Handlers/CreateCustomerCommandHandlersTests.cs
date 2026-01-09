using ControlePedidos.CustomerContext.Application.Commands;
using ControlePedidos.CustomerContext.Application.Commands.Create;
using ControlePedidos.CustomerContext.Application.Handlers;
using ControlePedidos.CustomerContext.Domain.Entities;
using FluentValidation;
using FluentAssertions;
using FluentValidation.Results;
using Moq;

namespace ControlePedidos.CustomerContext.Tests.Application.Handlers
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _mockRepository;
        private readonly Mock<IValidator<Customer>> _mockValidator;
        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _mockRepository = new Mock<ICustomerRepository>();
            _mockValidator = new Mock<IValidator<Customer>>();

            _handler = new CreateCustomerCommandHandler(
                _mockValidator.Object,
                _mockRepository.Object
            );

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult());
        }

        [Fact]
        public async Task HandleAsync_ShouldCreateCustomer_WhenCpfAndEmailAreUnique()
        {
            // ARRANGE (Configuração Específica do Teste)
            var command = new CreateCustomerCommand("12345678901", "Test User", "test@test.com");

            _mockRepository.Setup(r => r.GetByCpfAsync(command.Cpf))
                           .ReturnsAsync((Customer)null);

            _mockRepository.Setup(r => r.GetByEmailAsync(command.Email))
                           .ReturnsAsync((Customer)null);

            // ACT (Ação)
            await _handler.HandleAsync(command);

            // ASSERT (Verificação)
            // Verifica se o método CreateAsync foi chamado
            // com um cliente que tenha o CPF e Email corretos.
            _mockRepository.Verify(
                r => r.CreateAsync(It.Is<Customer>(
                    c => c.Cpf == command.Cpf && c.Email == command.Email
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowException_WhenCpfAlreadyExists()
        {
            // ARRANGE
            var command = new CreateCustomerCommand("11111111111", "Test User", "test@test.com");

            var cpfNormalizado = string.IsNullOrWhiteSpace(command.Cpf) ? null : command.Cpf;

            _mockRepository.Setup(r => r.GetByCpfAsync(cpfNormalizado!))
                           .ReturnsAsync(new Customer { Cpf = cpfNormalizado });

            // ACT
            Func<Task> act = async () => await _handler.HandleAsync(command);

            // ASSERT
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("CPF já cadastrado.");

            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // ARRANGE
            var command = new CreateCustomerCommand("12345678901", "Test User", "test@test.com");
            var emailNormalizado = string.IsNullOrWhiteSpace(command.Email) ? null : command.Email;

            _mockRepository.Setup(r => r.GetByCpfAsync(It.IsAny<string>()))
                           .ReturnsAsync((Customer?)null);

            _mockRepository.Setup(r => r.GetByEmailAsync(emailNormalizado!))
                           .ReturnsAsync(new Customer { Email = emailNormalizado });

            // ACT
            Func<Task> act = async () => await _handler.HandleAsync(command);

            // ASSERT
            await act.Should().ThrowAsync<Exception>()
                     .WithMessage("E-mail já cadastrado.");

            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowValidationException_WhenValidatorFails()
        {
            // ARRANGE
            var command = new CreateCustomerCommand("123", "Test User", "bad-email"); // Dados inválidos

            // Cria uma lista de erros de validação falsos
            var validationErrors = new List<ValidationFailure>
            {
                new ValidationFailure("Cpf", "CPF inválido."),
            };

            // Configura o validador para retornar a falha
            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new ValidationResult(validationErrors));

            // ACT
            Func<Task> act = async () => await _handler.HandleAsync(command);

            // ASSERT
            // Verifica se ele lança a exceção específica do FluentValidation
            await act.Should().ThrowAsync<ValidationException>();

            // Garante que o método CreateAsync NUNCA foi chamado
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }
    }
}