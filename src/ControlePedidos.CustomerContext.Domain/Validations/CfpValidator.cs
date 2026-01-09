namespace ControlePedidos.CustomerContext.Domain.Validations;

public static class CpfValidator
{
    public static bool IsValid(string? cpf) =>
        string.IsNullOrWhiteSpace(cpf) || cpf.Length == 11;
}