using Microsoft.AspNetCore.Http.HttpResults;
using PruebaTecnicaBack.Application.Users.Commands;
using PruebaTecnicaBack.Application.Users.Dtos;
using PruebaTecnicaBack.Application.Users.Queries;
using PruebaTecnicaBack.Filters;

namespace PruebaTecnicaBack.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Usuarios");

        group.MapGet("/", GetUsers);
        
        group.MapGet("/{id:int:min(1)}", GetUserById)
            .WithName("GetUserById");

        group.MapPost("/", CreateUser)
            .AddValidation<CreateUserCommand>();

        group.MapPost("/bulk", BulkCreateUsers)
            .AddValidation<BulkCreateUsersCommand>();

        group.MapPut("/{id:int:min(1)}", UpdateUser)
            .AddValidation<UpdateUserCommand>();

        group.MapDelete("/{id:int:min(1)}", DeleteUser);
    }

    private static async Task<Ok<List<UserDto>>> GetUsers(
        bool? isActive,
        GetUsersQueryHandler handler, 
        CancellationToken ct)
    {
        var users = await handler.Handle(new GetUsersQuery(isActive), ct);
        return TypedResults.Ok(users);
    }

    private static async Task<Ok<UserDto>> GetUserById(int id, GetUserByIdQueryHandler handler, CancellationToken ct) 
    {
        var user = await handler.Handle(new GetUserByIdQuery(id), ct);
        return TypedResults.Ok(user);
    }

    private static async Task<CreatedAtRoute<UserDto>> CreateUser(
        CreateUserCommand command,
        CreateUserCommandHandler handler,
        CancellationToken ct)
    {
        var res = await handler.Handle(command, ct);

        return TypedResults.CreatedAtRoute(
            res,
            "GetUserById",
            new { id = res.Id }
        );
    }

    private static async Task<IResult> UpdateUser(int id, UpdateUserCommand command, UpdateUserCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(id, command, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUser(int id, DeleteUserCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(new DeleteUserCommand(id), ct);
        return Results.NoContent();
    }

    private static async Task<Ok<BulkCreateUsersResult>> BulkCreateUsers(
        BulkCreateUsersCommand command, 
        BulkCreateUsersCommandHandler handler, 
        CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return TypedResults.Ok(result);
    }
}