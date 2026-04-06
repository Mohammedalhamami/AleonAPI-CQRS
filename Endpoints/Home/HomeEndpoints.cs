using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualBasic;

namespace AleonAPI.Endpoints.Home;

public static class HomeEndpoints
{

    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder route)
    {
       var homeGroup = route.MapGroup("/api/home").WithTags("Home");


       homeGroup.MapGet("/welcome", GetWelcomeResponse);

       

       return route;
    }

   private static async Task<Ok<WelcomeResponse>> GetWelcomeResponse(CancellationToken ct)
{
    var welcomeMessage = new WelcomeResponse
    {
        Message = "Hello World!",
        Version = "1.0.0",
        TimeOnly = DateTime.Now.ToString("HH:mm:ss")
    };

    await Task.CompletedTask; // placeholder (not recommended)
    return TypedResults.Ok(welcomeMessage);
}
    
}