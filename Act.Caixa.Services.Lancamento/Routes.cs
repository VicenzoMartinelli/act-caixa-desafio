using Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Act.Caixa.Services.Lancamento;

public static class Routes
{
    public static void UseApplicationRoutes(this WebApplication app)
    {
        app.MapGet(
                "/api/v1/lancamentos/{id}",
                async ([FromRoute] Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new FindLancamentoByIdQuery
                    {
                        Id = id
                    }, cancellationToken);

                    return Results.Ok(result);
                }
            )
            .WithOpenApi();
        
        app.MapGet(
                "/api/v1/lancamentos",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new FindLancamentosQuery(), cancellationToken);

                    return Results.Ok(result);
                }
            )
            .WithOpenApi();
        
        app.MapPost(
                "/api/v1/lancamentos",
                async ([FromBody] AddLancamentoCommand command, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var (ok, result) = await mediator.Send(command, cancellationToken);

                    if (!ok) return Results.BadRequest();

                    return Results.Created($"/api/v1/lancamentos/{result.GetId()}", result);
                }
            )
            .WithOpenApi();
        
        app.MapDelete(
                "/api/v1/lancamento/{id}", 
                async ([FromRoute] Guid id, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var (ok, _) = await mediator.Send(new DeleteLancamentoCommand { Id = id }, cancellationToken);

                    if (!ok)
                        return Results.BadRequest();
        
                    return Results.NoContent();
                })
            .WithOpenApi();
    }
}