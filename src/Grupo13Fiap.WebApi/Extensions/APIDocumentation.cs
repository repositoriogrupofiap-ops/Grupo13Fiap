using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
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
                    Title       = ApiInfo.Title,
                    Version     = ApiInfo.Versions.V1,
                    Description = ApiInfo.Description,
                };
                document.Servers =
                [
                    new() { Url = ApiInfo.Server.Production, Description = "Servidor de Produção" },
                    new() { Url = ApiInfo.Server.Local,       Description = "Servidor Local"       },
                ];
                document.ExternalDocs = new()
                {
                    Description = ApiInfo.Suport.Name,
                    Url         = new Uri(ApiInfo.Suport.Url)
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "Bearer",
                    BearerFormat = "JWT",
                    In           = ParameterLocation.Header,
                    Description  = "Enter your JWT token"
                };

                return Task.CompletedTask;
            });

            o.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var actionDescriptor = context.Description.ActionDescriptor as ControllerActionDescriptor;
                if (actionDescriptor is null)
                    return Task.CompletedTask;

                var isAnonymous =
                    actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>().Any()
                    || actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true).OfType<AllowAnonymousAttribute>().Any();

                var requiresAuth =
                    !isAnonymous && (
                        actionDescriptor.ControllerTypeInfo.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any()
                        || actionDescriptor.MethodInfo.GetCustomAttributes(inherit: true).OfType<AuthorizeAttribute>().Any()
                    );

                if (requiresAuth)
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
