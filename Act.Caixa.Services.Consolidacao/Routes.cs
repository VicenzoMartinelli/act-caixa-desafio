using Act.Caixa.Consolidacao.Features.Consolidacao.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Act.Caixa.Consolidacao;

public static class Routes
{
    public static void UseApplicationRoutes(this WebApplication app)
    {
        app.MapGet(
                "/api/v1/consolidacoes",
                async ([FromQuery] DateOnly? filtroData, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new FindConsolidacoesQuery
                    {
                        FiltroData = filtroData
                    }, cancellationToken);

                    return Results.Ok(result);
                }
            )
            .WithOpenApi();
    }
}