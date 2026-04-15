using System.Reflection.Metadata;
using AleonAPI.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;

namespace AleonAPI.Extensions.cs
{
    public static class OpenAPISwaggerExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Aeon Registry API",
                    Version = "v1",
                    Description = """

                                  <img src="/images/AeonRegistryLogoBLK.png" height="120" />

                                  ## Aeon Research Division

                                  Internal API for managing recovered artifacts and research data.Provides secure access for field researchers and analysts.### Key Features:
                                  - Site and Artifact Catalog
                                  - Research record submissions
                                  - Secure media storage
                                  - User role management

                                  """,
                    Contact = new OpenApiContact
                    {
                        Name = "Aeon Registry Team",
                        Url = new Uri("https://learn.coderfoundry.com"),
                        Email = "support@coderfoundry.com"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid JWT token."
                });

                c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });

                c.UseInlineDefinitionsForEnums();
                c.SchemaFilter<EnumStringFilter>();

                string[] hiddenEndpoints =
                [
                    "api/auth/register",
                    "api/auth/refresh",
                    "api/auth/confirmemail",
                    "api/auth/resendconfirmationemail",
                    "api/auth/forgotpassword",
                    "api/auth/resetpassword",
                    "api/auth/manage",
                    "api/auth/manage/info",
                    "api/auth/manage/2fa"
                ];

                c.DocInclusionPredicate((docName, ApiDescription) =>
                {
                    var path = ApiDescription.RelativePath?.ToLowerInvariant();
                    if (path is null) return false;

                    if (hiddenEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    return true;
                });
            });

            return services;
        }
    }
}