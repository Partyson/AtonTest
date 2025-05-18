using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AtonTest.Helpers;

public class RequireOwnersAttribute : Attribute, IAsyncActionFilter
{
    private readonly string routeParamName;

    public RequireOwnersAttribute(string routeParamName)
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

        if (context.ActionArguments.TryGetValue(routeParamName, out var value) &&
            value?.ToString() == userLogin)
        {
            await next();
            return;
        }

        context.Result = new ObjectResult("Это не ваш профиль.")
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}