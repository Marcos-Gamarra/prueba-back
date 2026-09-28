using FluentValidation;

namespace PruebaTecnicaBack.Application.Common.Validators;

public static class CustomValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> NotEmptyOrWhiteSpace<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string message)
    {
        return ruleBuilder
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage(message);
    }

    public static IRuleBuilderOptions<T, string> ValidCustomEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Email es requerido.")
            .Matches(RegexPatterns.EmailRegex())
            .WithMessage("Formato de email no válido.");
    }

    public static IRuleBuilderOptions<T, string?> ValidCustomPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial (no alfanumérico).");
    }
}