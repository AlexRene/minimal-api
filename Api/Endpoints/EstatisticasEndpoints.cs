using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Repositories;

namespace MinimalApi.Endpoints;

public static class EstatisticasEndpoints
{
    public static void MapEstatisticasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/estatisticas")
                      .WithTags("Estatísticas")
                      .WithOpenApi();

        group.MapGet("/", ObterEstatisticasGerais)
             .RequireAuthorization()
             .WithName("ObterEstatisticasGerais")
             .WithSummary("Obter estatísticas gerais do sistema")
             .WithDescription("Retorna estatísticas sobre administradores e veículos");

        group.MapGet("/veiculos", ObterEstatisticasVeiculos)
             .RequireAuthorization()
             .WithName("ObterEstatisticasVeiculos")
             .WithSummary("Obter estatísticas dos veículos")
             .WithDescription("Retorna estatísticas detalhadas sobre os veículos");
    }

    private static async Task<IResult> ObterEstatisticasGerais(
        IAdministradorRepository administradorRepository,
        IVeiculoRepository veiculoRepository)
    {
        try
        {
            var totalAdministradores = await administradorRepository.CountAsync();
            var totalVeiculos = await veiculoRepository.CountAsync();

            var estatisticas = new
            {
                TotalAdministradores = totalAdministradores,
                TotalVeiculos = totalVeiculos,
                DataConsulta = DateTime.UtcNow,
                Status = "Ativo"
            };

            return Results.Ok(estatisticas);
        }
        catch
        {
            return Results.Problem("Erro ao obter estatísticas gerais");
        }
    }

    private static async Task<IResult> ObterEstatisticasVeiculos(
        IVeiculoRepository veiculoRepository)
    {
        try
        {
            var totalVeiculos = await veiculoRepository.CountAsync();
            
            // Aqui você poderia implementar queries mais complexas para estatísticas
            // Por exemplo, veículos por ano, por marca, etc.
            
            var estatisticas = new
            {
                TotalVeiculos = totalVeiculos,
                DataConsulta = DateTime.UtcNow,
                Observacao = "Estatísticas básicas implementadas. Estatísticas avançadas podem ser adicionadas conforme necessário."
            };

            return Results.Ok(estatisticas);
        }
        catch
        {
            return Results.Problem("Erro ao obter estatísticas dos veículos");
        }
    }
}
