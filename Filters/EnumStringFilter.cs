using AleonAPI.Enums;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AleonAPI.Filters;

public class EnumStringFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(string) && context.MemberInfo?.Name == "Type")
        {
            schema.Description = "Allowed values: " +
                         string.Join(", ", Enum.GetNames(typeof(ArtifactType)));
        }
        else if (context.Type == typeof(string) && context.MemberInfo?.Name == "CatalogStatus")
        {
            schema.Description = "Allowed values: " +
                         string.Join(", ", Enum.GetNames(typeof(CatalogStatus)));
        }
    }
}
