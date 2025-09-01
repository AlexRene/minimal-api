using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Endpoints;

public static class VeiculoEndpoints
{
    public static void MapVeiculoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/veiculos")
                      .WithTags("Veiculos")
                      .WithOpenApi();

        group.MapGet("/", ListarVeiculos)
             .RequireAuthorization()
             .WithName("ListarVeiculos")
             .WithSummary("Listar todos os veículos")
             .WithDescription("Retorna uma lista paginada de veículos");

        group.MapGet("/filtros", ListarVeiculosComFiltrosAvancados)
             .RequireAuthorization()
             .WithName("ListarVeiculosComFiltrosAvancados")
             .WithSummary("Listar veículos com filtros avançados")
             .WithDescription("Retorna uma lista paginada de veículos com filtros e ordenação");

        group.MapGet("/marca/{marca}", BuscarVeiculosPorMarca)
             .RequireAuthorization()
             .WithName("BuscarVeiculosPorMarca")
             .WithSummary("Buscar veículos por marca")
             .WithDescription("Retorna todos os veículos de uma marca específica");

        group.MapGet("/ano/{ano}", BuscarVeiculosPorAno)
             .RequireAuthorization()
             .WithName("BuscarVeiculosPorAno")
             .WithSummary("Buscar veículos por ano")
             .WithDescription("Retorna todos os veículos de um ano específico");

        group.MapGet("/nome/{nome}", BuscarVeiculosPorNome)
             .RequireAuthorization()
             .WithName("BuscarVeiculosPorNome")
             .WithSummary("Buscar veículos por nome")
             .WithDescription("Retorna todos os veículos que contenham o nome especificado");

        group.MapGet("/{id}", BuscarVeiculoPorId)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
             .WithName("BuscarVeiculoPorId")
             .WithSummary("Buscar veículo por ID")
             .WithDescription("Retorna um veículo específico pelo ID");

        group.MapPost("/", CriarVeiculo)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
             .WithName("CriarVeiculo")
             .WithSummary("Criar novo veículo")
             .WithDescription("Cria um novo veículo no sistema");

        group.MapPut("/{id}", AtualizarVeiculo)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
             .WithName("AtualizarVeiculo")
             .WithSummary("Atualizar veículo")
             .WithDescription("Atualiza um veículo existente");

        group.MapDelete("/{id}", DeletarVeiculo)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
             .WithName("DeletarVeiculo")
             .WithSummary("Deletar veículo")
             .WithDescription("Remove um veículo do sistema");
    }

    private static ErrosDeValidacao ValidarVeiculoDTO(VeiculoDTO veiculoDTO)
    {
        var validacao = new ErrosDeValidacao{
            Mensagens = new List<string>()
        };

        if(string.IsNullOrEmpty(veiculoDTO.Nome))
            validacao.Mensagens.Add("O nome não pode ser vazio");

        if(string.IsNullOrEmpty(veiculoDTO.Marca))
            validacao.Mensagens.Add("A Marca não pode ficar em branco");

        if(veiculoDTO.Ano < 1950)
            validacao.Mensagens.Add("Veículo muito antigo, aceito somente anos superiores a 1950");

        if(veiculoDTO.Ano > DateTime.Now.Year + 1)
            validacao.Mensagens.Add("Ano do veículo não pode ser futuro");

        return validacao;
    }

    private static async Task<IResult> ListarVeiculos(
        [FromQuery] int? pagina, 
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculos = await veiculoServico.Todos(pagina);
            return Results.Ok(veiculos);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículos");
        }
    }

    private static async Task<IResult> ListarVeiculosComFiltrosAvancados(
        [FromQuery] VeiculoFiltroDTO filtro,
        IVeiculoServico veiculoServico)
    {
        try
        {
            var resultado = await veiculoServico.TodosComFiltrosAvancados(filtro);
            return Results.Ok(resultado);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículos com filtros");
        }
    }

    private static async Task<IResult> BuscarVeiculosPorMarca(
        [FromRoute] string marca,
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculos = await veiculoServico.BuscarPorMarca(marca);
            return Results.Ok(veiculos);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículos por marca");
        }
    }

    private static async Task<IResult> BuscarVeiculosPorAno(
        [FromRoute] int ano,
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculos = await veiculoServico.BuscarPorAno(ano);
            return Results.Ok(veiculos);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículos por ano");
        }
    }

    private static async Task<IResult> BuscarVeiculosPorNome(
        [FromRoute] string nome,
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculos = await veiculoServico.BuscarPorNome(nome);
            return Results.Ok(veiculos);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículos por nome");
        }
    }

    private static async Task<IResult> BuscarVeiculoPorId(
        [FromRoute] int id, 
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculo = await veiculoServico.BuscaPorId(id);
            if(veiculo == null) 
                return Results.NotFound();
                
            return Results.Ok(veiculo);
        }
        catch
        {
            return Results.Problem("Erro ao buscar veículo");
        }
    }

    private static async Task<IResult> CriarVeiculo(
        [FromBody] VeiculoDTO veiculoDTO, 
        IVeiculoServico veiculoServico)
    {
        try
        {
            var validacao = ValidarVeiculoDTO(veiculoDTO);
            if(validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);
            
            var veiculo = new Veiculo{
                Nome = veiculoDTO.Nome,
                Marca = veiculoDTO.Marca,
                Ano = veiculoDTO.Ano
            };
            
            await veiculoServico.Incluir(veiculo);

            return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
        }
        catch
        {
            return Results.Problem("Erro ao criar veículo");
        }
    }

    private static async Task<IResult> AtualizarVeiculo(
        [FromRoute] int id, 
        [FromBody] VeiculoDTO veiculoDTO, 
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculo = await veiculoServico.BuscaPorId(id);
            if(veiculo == null) 
                return Results.NotFound();
            
            var validacao = ValidarVeiculoDTO(veiculoDTO);
            if(validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);
            
            veiculo.Nome = veiculoDTO.Nome;
            veiculo.Marca = veiculoDTO.Marca;
            veiculo.Ano = veiculoDTO.Ano;

            await veiculoServico.Atualizar(veiculo);

            return Results.Ok(veiculo);
        }
        catch
        {
            return Results.Problem("Erro ao atualizar veículo");
        }
    }

    private static async Task<IResult> DeletarVeiculo(
        [FromRoute] int id, 
        IVeiculoServico veiculoServico)
    {
        try
        {
            var veiculo = await veiculoServico.BuscaPorId(id);
            if(veiculo == null) 
                return Results.NotFound();

            await veiculoServico.Apagar(veiculo);

            return Results.NoContent();
        }
        catch
        {
            return Results.Problem("Erro ao deletar veículo");
        }
    }
}
