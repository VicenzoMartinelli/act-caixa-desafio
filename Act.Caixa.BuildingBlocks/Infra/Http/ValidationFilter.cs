using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Act.Caixa.BuildingBlocks.Infra.Http;

public class ValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(e => new 
            {
                Campo = e.Key,
                Mensagens = e.Value.Errors.Select(err => err.ErrorMessage)
            });

        var resultado = new
        {
            Sucesso = false,
            Mensagem = "Falha na validação de entrada",
            Erros = errors
        };

        context.Result = new BadRequestObjectResult(resultado);
    }
}
