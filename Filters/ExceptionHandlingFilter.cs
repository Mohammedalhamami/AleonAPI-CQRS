using System;

namespace AleonAPI.Filters;

public class ExceptionHandlingFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            var env = context.HttpContext.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
            // Log the exception (you can use a logging framework here)
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Return a generic error response
            return Results.Problem(
                detail: env.IsDevelopment() ? ex.ToString() : null, 
                title: "An unexpected error occurred.",
                
                statusCode: 500);
        }
    }
}
