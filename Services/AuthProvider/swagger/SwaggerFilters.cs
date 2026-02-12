using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace AuthProvider.Swagger
{
    /// <summary>
    /// Custom schema filter to enhance Swagger documentation
    /// </summary>
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null) return;

            // Add examples and descriptions for common properties
            foreach (var property in schema.Properties)
            {
                switch (property.Key.ToLower())
                {
                    case "id":
                        property.Value.Description = "Unique identifier";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
                        break;
                    case "name":
                        property.Value.Description = "Name of the entity";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("Sample Name");
                        break;
                    case "title":
                        property.Value.Description = "Title of the entity";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("Sample Title");
                        break;
                    case "description":
                        property.Value.Description = "Detailed description";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("Sample description");
                        break;
                    case "code":
                        property.Value.Description = "Unique code identifier";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("CODE001");
                        break;
                    case "email":
                        property.Value.Description = "Email address";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("user@example.com");
                        break;
                    case "phone":
                    case "phonenumber":
                        property.Value.Description = "Phone number";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("+1234567890");
                        break;
                    case "isactive":
                        property.Value.Description = "Indicates if the entity is active";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiBoolean(true);
                        break;
                    case "createdat":
                        property.Value.Description = "Date and time when the entity was created";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15T10:30:00Z");
                        break;
                    case "updatedat":
                        property.Value.Description = "Date and time when the entity was last updated";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15T10:30:00Z");
                        break;
                    case "pagenumber":
                        property.Value.Description = "Page number for pagination (starts from 1)";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
                        break;
                    case "pagesize":
                        property.Value.Description = "Number of items per page (1-100)";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiInteger(10);
                        break;
                    case "totalcount":
                        property.Value.Description = "Total number of items available";
                        property.Value.Example = new Microsoft.OpenApi.Any.OpenApiInteger(100);
                        break;
                }
            }

            // Add descriptions for enum types
            if (context.Type.IsEnum)
            {
                schema.Description = $"Enumeration values for {context.Type.Name}";
                var enumValues = Enum.GetValues(context.Type);
                var enumNames = Enum.GetNames(context.Type);
                
                var enumDescriptions = new List<string>();
                for (int i = 0; i < enumNames.Length; i++)
                {
                    enumDescriptions.Add($"{(int)enumValues.GetValue(i)} = {enumNames[i]}");
                }
                
                if (enumDescriptions.Any())
                {
                    schema.Description += $"\n\nAvailable values:\n{string.Join("\n", enumDescriptions)}";
                }
            }
        }
    }

    /// <summary>
    /// Custom operation filter to enhance API endpoint documentation
    /// </summary>
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Add common response codes for all endpoints
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Unauthorized - JWT token is missing or invalid"
                });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Forbidden - User doesn't have permission to access this resource"
                });
            }

            // Add specific examples for Customer Portal endpoints
            if (context.ApiDescription.RelativePath?.Contains("customerportal") == true)
            {
                // Add common Customer Portal response examples
                if (operation.Responses.ContainsKey("500"))
                {
                    operation.Responses["500"].Description = "Internal Server Error - Database connection or server error";
                }

                // Add pagination parameters documentation for GET endpoints
                if (context.ApiDescription.HttpMethod?.ToUpper() == "GET" && 
                    operation.Parameters?.Any(p => p.Name == "pageNumber") == true)
                {
                    var pageNumberParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageNumber");
                    if (pageNumberParam != null)
                    {
                        pageNumberParam.Description = "Page number for pagination. Must be greater than 0. Default is 1.";
                        pageNumberParam.Example = new Microsoft.OpenApi.Any.OpenApiInteger(1);
                    }

                    var pageSizeParam = operation.Parameters.FirstOrDefault(p => p.Name == "pageSize");
                    if (pageSizeParam != null)
                    {
                        pageSizeParam.Description = "Number of items per page. Must be between 1 and 100. Default is 10.";
                        pageSizeParam.Example = new Microsoft.OpenApi.Any.OpenApiInteger(10);
                    }

                    var searchParam = operation.Parameters.FirstOrDefault(p => p.Name == "searchTerm");
                    if (searchParam != null)
                    {
                        searchParam.Description = "Search term to filter results. Searches across multiple fields.";
                        searchParam.Example = new Microsoft.OpenApi.Any.OpenApiString("search term");
                    }

                    var isActiveParam = operation.Parameters.FirstOrDefault(p => p.Name == "isActive");
                    if (isActiveParam != null)
                    {
                        isActiveParam.Description = "Filter by active status. True for active entities, false for inactive.";
                        isActiveParam.Example = new Microsoft.OpenApi.Any.OpenApiBoolean(true);
                    }
                }
            }

            // Set operation summary based on HTTP method and controller
            if (string.IsNullOrEmpty(operation.Summary))
            {
                var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];
                var actionName = context.ApiDescription.ActionDescriptor.RouteValues["action"];
                var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();

                operation.Summary = httpMethod switch
                {
                    "GET" => actionName?.ToLower() switch
                    {
                        "search" => $"Search {controllerName}",
                        _ when actionName?.Contains("Get") == true && context.ApiDescription.RelativePath?.Contains("{id}") == true => $"Get {controllerName?.TrimEnd('s')} by ID",
                        _ => $"Get {controllerName} list"
                    },
                    "POST" => $"Create new {controllerName?.TrimEnd('s')}",
                    "PUT" => $"Update {controllerName?.TrimEnd('s')}",
                    "PATCH" => $"Update {controllerName?.TrimEnd('s')} status",
                    "DELETE" => $"Delete {controllerName?.TrimEnd('s')}",
                    _ => operation.Summary
                };
            }
        }
    }
}