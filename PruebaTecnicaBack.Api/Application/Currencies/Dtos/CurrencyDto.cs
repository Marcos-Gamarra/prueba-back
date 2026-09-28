using System.ComponentModel;

namespace PruebaTecnicaBack.Application.Currencies.Dtos;

public record CurrencyDto(
    [property: Description("ID de la moneda")]
    int Id,
    [property: Description("Código de la moneda")]
    string Code,
    [property: Description("Nombre de la moneda")]
    string Name,
    [property: Description("Tasa de conversión a la moneda base")]
    decimal RateToBase
);