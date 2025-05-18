using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtonTest.CustomAttributes;

public class RequireOwnershipOrAdminAttribute : Attribute, IAsyncActionFilter
{
    private readonly string routeParamName;

    public RequireOwnershipOrAdminAttribute(string routeParamName)
    {
        this.routeParamName = routeParamName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user.FindFirst("IsRevoked")?.Value == "true")
        {
            context.Result = new ObjectResult("Вы забанены")
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        var userLogin = user.FindFirst("Login")?.Value;

        if (user.FindFirst(ClaimTypes.Role)?.Value == "Admin")
        {
            await next(); 
            return;
        }

        if (context.ActionArguments.TryGetValue(routeParamName, out var value) &&
            value?.ToString() == userLogin)
        {
            await next();
            return;
        }

        context.Result = new ObjectResult("Можно изменять только свой профиль")
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}