using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.BuildingBlocks.Infra.Http;
using Act.Caixa.BuildingBlocks.Infra.MessageBus.Configuration;
using Act.Caixa.Consolidacao;
using Act.Caixa.Consolidacao.Features.Consolidacao.Consumers;
using Act.Caixa.Consolidacao.Features.Consolidacao.Queries;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilterAttribute>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<FindConsolidacoesQuery>();
});
builder.Services.AddDatabaseConfiguration();
builder.Services.AddMessageBus<AtualizaConsolidacaoDiariaConsumer>(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseApplicationRoutes();
app.Run();