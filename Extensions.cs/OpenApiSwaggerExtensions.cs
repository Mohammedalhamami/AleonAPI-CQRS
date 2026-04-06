using Microsoft.OpenApi;
namespace AleonAPI.Extensions.cs;

public static class OpenApiSwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
       services.AddEndpointsApiExplorer();
       services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo
           {
               Title = "AleonAPI",
               Version = "v1",
               Description = "AleonAPI is a RESTful API for the Aleon platform",
               Contact = new OpenApiContact
               {
                   Name = "Aleon",
                   Email = "info@aleon.com"
               }
           });
       });
        return services;
    }
}