namespace ControlePedidos.CustomerContext.Domain.Validations;

public static class CpfValidator
{
    public static bool IsValid(string cpf)
    {
        return !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11; // Assumindo CPF sem pontos/traços
    }
}