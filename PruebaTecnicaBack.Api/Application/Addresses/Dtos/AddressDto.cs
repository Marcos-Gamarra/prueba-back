namespace PruebaTecnicaBack.Application.Addresses.Dtos;
using System.ComponentModel;

public record AddressDto(
    [property: Description("Identificador único de la dirección")]
    int Id,
    [property: Description("Identificador único del usuario al que pertenece la dirección")]
    int UserId,
    [property: Description("Calle de la dirección")]
    string Street,
    [property: Description("Ciudad de la dirección")]
    string City,
    [property: Description("País de la dirección")]
    string Country,
    [property: Description("Código postal de la dirección")]
    string? ZipCode
);
