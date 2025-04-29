namespace BloodBank.Infrastructure.Services.Address.ViaCep.Models;

public record ViaCepResponse(
    string? Cep,
    string? Logradouro,
    string? Bairro,
    string? Localidade,
    string? Uf,
    string? Erro
);
