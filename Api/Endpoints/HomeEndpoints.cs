using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Endpoints;

public static class HomeEndpoints
{
    public static void MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => Results.Json(new Home()))
           .AllowAnonymous()
           .WithTags("Home")
           .WithName("Home")
           .WithSummary("Endpoint inicial da API")
           .WithDescription("Retorna informações básicas sobre a API");
    }
}
