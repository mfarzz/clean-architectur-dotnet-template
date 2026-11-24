using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MyCleanApi.WebApi.Infrastructure;

public sealed class ApiDocumentationTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info.Title = "MyCleanApi API";
        document.Info.Version = "v1";
        document.Info.Description = "Clean Architecture API reference. Use Authorize to enter a Bearer token. " +
            "Sample endpoints are public; token validation must be configured in the API before protecting endpoints.";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste the access token only, without the Bearer prefix."
        };

        return Task.CompletedTask;
    }

    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any() &&
            !metadata.OfType<IAllowAnonymous>().Any();

        // An empty alternative keeps public endpoints optional while allowing
        // Swagger UI to attach a token supplied through Authorize.
        operation.Security = [new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        }];
        if (!requiresAuthorization)
        {
            operation.Security.Add(new OpenApiSecurityRequirement());
        }

        return Task.CompletedTask;
    }
}
