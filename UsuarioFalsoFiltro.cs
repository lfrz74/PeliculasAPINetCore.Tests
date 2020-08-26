using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests
{
    public class UsuarioFalsoFiltro : IAsyncActionFilter
    {
        protected string usuarioPorDefectoId = "655f5207-6a7c-4c9e-9941-d173311ff2b0";
        protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)

            }, "prueba"));

            await next();
        }
    }
}
