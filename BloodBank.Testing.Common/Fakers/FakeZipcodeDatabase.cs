using BloodBank.Infrastructure.Services.Address.ViaCep.Models;

namespace BloodBank.Testing.Common.Fakers;

public static class FakeZipcodeDatabase
{
   public static readonly Dictionary<string, ViaCepResponse> ValidZipCodes = new()
{
    ["01001000"] = new("01001000", "Praça da Sé", "Sé", "São Paulo", "SP", null),
    ["30140071"] = new("30140071", "Avenida Afonso Pena", "Centro", "Belo Horizonte", "MG", null),
    ["70040900"] = new("70040900", "Praça dos Três Poderes", "Zona Cívico-Administrativa", "Brasília", "DF", null),
    ["88010001"] = new("88010001", "Rua Felipe Schmidt", "Centro", "Florianópolis", "SC", null),
    ["80010020"] = new("80010020", "Rua Marechal Deodoro", "Centro", "Curitiba", "PR", null),
    ["40020900"] = new("40020900", "Avenida Sete de Setembro", "Dois de Julho", "Salvador", "BA", null),
    ["59015100"] = new("59015100", "Rua Apodi", "Tirol", "Natal", "RN", null),
    ["64000010"] = new("64000010", "Avenida Frei Serafim", "Centro", "Teresina", "PI", null),
    ["69005010"] = new("69005010", "Rua Joaquim Nabuco", "Centro", "Manaus", "AM", null),
    ["69020030"] = new("69020030", "Rua Ramos Ferreira", "Centro", "Manaus", "AM", null),
    ["60170110"] = new("60170110", "Rua Tibúrcio Cavalcante", "Meireles", "Fortaleza", "CE", null),
    ["64001280"] = new("64001280", "Rua Rui Barbosa", "Centro", "Teresina", "PI", null),
    ["59064000"] = new("59064000", "Rua Presidente Sarmento", "Lagoa Nova", "Natal", "RN", null),
    ["69900076"] = new("69900076", "Rua Floriano Peixoto", "Centro", "Rio Branco", "AC", null),
    ["76801176"] = new("76801176", "Avenida Sete de Setembro", "Centro", "Porto Velho", "RO", null),
    ["72870110"] = new("72870110", "Rua 02", "Novo Gama", "Novo Gama", "GO", null),
    ["65010680"] = new("65010680", "Avenida Pedro II", "Centro", "São Luís", "MA", null),
    ["68900010"] = new("68900010", "Rua Cândido Mendes", "Centro", "Macapá", "AP", null),
    ["57020380"] = new("57020380", "Rua do Sol", "Centro", "Maceió", "AL", null),
    ["50010000"] = new("50010000", "Avenida Guararapes", "Santo Antônio", "Recife", "PE", null),
    ["64018620"] = new("64018620", "Rua Visconde da Parnaíba", "Cristo Rei", "Teresina", "PI", null),
    ["77001008"] = new("77001008", "Quadra 103 Norte Avenida LO 02", "Plano Diretor Norte", "Palmas", "TO", null),
    ["66055010"] = new("66055010", "Avenida Nazaré", "Nazaré", "Belém", "PA", null),
    ["57010740"] = new("57010740", "Rua Barão de Atalaia", "Centro", "Maceió", "AL", null),
    ["64000070"] = new("64000070", "Rua Álvaro Mendes", "Centro", "Teresina", "PI", null),
    ["59020001"] = new("59020001", "Avenida Deodoro da Fonseca", "Cidade Alta", "Natal", "RN", null),
    ["69900902"] = new("69900902", "Avenida Brasil", "Centro", "Rio Branco", "AC", null),
    ["69020400"] = new("69020400", "Rua Ferreira Pena", "Centro", "Manaus", "AM", null),
    ["01011900"] = new("01011900", "Rua João Brícola", "Centro", "São Paulo", "SP", null),
    ["30130010"] = new("30130010", "Avenida Álvares Cabral", "Centro", "Belo Horizonte", "MG", null)
};
    
    public static string GetRandomCep()
    {
        return ValidZipCodes.Keys.ElementAt(Random.Shared.Next(ValidZipCodes.Count));
    }


    public static ViaCepResponse GetResponse(string zipCode)
    {
        return ValidZipCodes.TryGetValue(zipCode, out var response)
            ? response
            : throw new KeyNotFoundException($"ZipCode '{zipCode}' not found in fake database.");
    }
}