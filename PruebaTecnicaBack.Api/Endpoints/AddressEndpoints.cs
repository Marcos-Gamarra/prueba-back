using Microsoft.AspNetCore.Http.HttpResults;
using PruebaTecnicaBack.Application.Addresses.Commands;
using PruebaTecnicaBack.Application.Addresses.Dtos;
using PruebaTecnicaBack.Application.Addresses.Queries;
using PruebaTecnicaBack.Filters;

namespace PruebaTecnicaBack.Endpoints;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this IEndpointRouteBuilder app)
    {
        var usersGroup = app
            .MapGroup("/users/{userId:int:min(1)}/addresses")
            .WithTags("Direcciones");

        usersGroup
            .MapPost("/", CreateAddress)
            .AddValidation<CreateAddressCommand>();

        usersGroup
            .MapGet("/", GetAddresses);

        var addressGroup = app
            .MapGroup("/addresses")
            .WithTags("Direcciones");

        addressGroup
            .MapPut("/{id:int:min(1)}", UpdateAddress)
            .AddValidation<UpdateAddressCommand>();

        addressGroup
            .MapDelete("/{id:int:min(1)}", DeleteAddress);
    }

    private static async Task<Created<AddressDto>> CreateAddress(
        int userId,
        CreateAddressCommand command,
        CreateAddressCommandHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(userId, command, ct);
        return TypedResults.Created($"/users/{userId}/addresses", res);
    }

    private static async Task<Ok<List<AddressDto>>> GetAddresses(
        int userId,
        GetAddressesQueryHandler handler,
        CancellationToken ct)
    {
        var addresses = await handler.Handle(new GetAddressesQuery(userId), ct);
        return TypedResults.Ok(addresses);
    }

    private static async Task<IResult> UpdateAddress(
        int id,
        UpdateAddressCommand command,
        UpdateAddressCommandHandler handler,
        CancellationToken ct)
    {
        await handler.Handle(id, command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteAddress(
        int id,
        DeleteAddressCommandHandler handler,
        CancellationToken ct)
    {
        await handler.Handle(new DeleteAddressCommand(id), ct);
        return Results.NoContent();
    }
}