using FluentValidation;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Sqlite;
using PruebaTecnicaBack.Application.Addresses.Commands;
using PruebaTecnicaBack.Application.Addresses.Queries;
using PruebaTecnicaBack.Application.Currencies.Commands;
using PruebaTecnicaBack.Application.Currencies.Queries;
using PruebaTecnicaBack.Application.CurrencyConversion;
using PruebaTecnicaBack.Application.Interfaces;
using PruebaTecnicaBack.Application.Users.Commands;
using PruebaTecnicaBack.Application.Users.Queries;
using PruebaTecnicaBack.ExceptionHandlers;
using PruebaTecnicaBack.Infrastructure.Persistence;
using PruebaTecnicaBack.Infrastructure.Security;
using PruebaTecnicaBack.OpenApi;
using PruebaTecnicaBack.Options;

namespace PruebaTecnicaBack;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Users
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<DeleteUserCommandHandler>();
        services.AddScoped<UpdateUserCommandHandler>();
        services.AddScoped<BulkCreateUsersCommandHandler>();
        services.AddScoped<GetUsersQueryHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();

        // Addresses
        services.AddScoped<CreateAddressCommandHandler>();
        services.AddScoped<GetAddressesQueryHandler>();
        services.AddScoped<UpdateAddressCommandHandler>();
        services.AddScoped<DeleteAddressCommandHandler>();

        // Currencies
        services.AddScoped<CreateCurrencyCommandHandler>();
        services.AddScoped<ConvertCurrencyCommandHandler>();
        services.AddScoped<GetCurrenciesQueryHandler>();

        services.Configure<BulkUsersOptions>(
            configuration.GetSection(BulkUsersOptions.SectionName));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString)
                   .UseExceptionProcessor());

        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>();
        });

        services.Configure<ApiKeySecurityOptions>(
            configuration.GetSection(ApiKeySecurityOptions.SectionName));

        services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<DatabaseExceptionHandler>();
        services.AddExceptionHandler<GlobalFallbackExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
