using System.Text.Json.Serialization;
using Act.Caixa.BuildingBlocks.Infra.Db;
using Act.Caixa.BuildingBlocks.Infra.Http;
using Act.Caixa.BuildingBlocks.Infra.MessageBus.Configuration;
using Act.Caixa.Services.Lancamento;
using Act.Caixa.Services.Lancamento.Features.Lancamentos.Commands;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

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
    cfg.RegisterServicesFromAssemblyContaining<AddLancamentoCommand>();
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddDatabaseConfiguration();
builder.Services.AddMessageBus(builder.Configuration);
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseApplicationRoutes();
app.Run();