using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;

namespace Grupo13Fiap.WebApi.Extensions;

public static class APIDocumentation
{
    public static void AddAPIDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi("v1", o =>
        {
            o.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = ApiInfo.Title,
                    Version = ApiInfo.Versions.V1,
                    Description = ApiInfo.Description,
                };
                document.Servers =
                [
                    new()
                    {
                        Url = ApiInfo.Server.Production,
                        Description = "Servidor de Produção"
                    },
                    new()
                    {
                        Url = ApiInfo.Server.Local,
                        Description = "Servidor Local"
                    },
                ];
                document.ExternalDocs = new()
                {
                    Description = ApiInfo.Suport.Name,
                    Url = new Uri(ApiInfo.Suport.Url)
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Insira o token JWT obtido no endpoint de login."
                });
                return Task.CompletedTask;
            });

            o.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var hasAuthorize = context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<IAuthorizeData>()
                    .Any();

                if (hasAuthorize)
                {
                    operation.Security =
                    [
                        new OpenApiSecurityRequirement
                        {
                            [new OpenApiSecuritySchemeReference("Bearer", null)] = []
                        }
                    ];
                }

                return Task.CompletedTask;
            });
        });
    }
}
