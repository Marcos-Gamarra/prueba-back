namespace PruebaTecnicaBack.Application.Users.Dtos;

using System.ComponentModel;

public record UserDto(
    [property: Description("Identificador único del usuario")]
    int Id,
    [property: Description("Nombre del usuario")]
    string Name,
    [property: Description("Dirección de correo electrónico del usuario")]
    string Email,
    [property: Description("Indica si el usuario está activo")]
    bool IsActive
);