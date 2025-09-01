using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Configuracoes;
using MinimalApi.DTOs;

namespace MinimalApi.Endpoints;

public static class AdministradorEndpoints
{
    public static void MapAdministradorEndpoints(this IEndpointRouteBuilder app, IConfiguration configuration)
    {
        var group = app.MapGroup("/administradores")
                      .WithTags("Administradores")
                      .WithOpenApi();

        // Login - Endpoint público
        group.MapPost("/login", Login)
             .AllowAnonymous()
             .WithName("Login")
             .WithSummary("Realizar login de administrador")
             .WithDescription("Autentica um administrador e retorna um token JWT");

        // Endpoints protegidos
        group.MapGet("/", ListarAdministradores)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
             .WithName("ListarAdministradores")
             .WithSummary("Listar todos os administradores")
             .WithDescription("Retorna uma lista paginada de administradores");

        group.MapGet("/{id}", BuscarAdministradorPorId)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
             .WithName("BuscarAdministradorPorId")
             .WithSummary("Buscar administrador por ID")
             .WithDescription("Retorna um administrador específico pelo ID");

        group.MapPost("/", CriarAdministrador)
             .RequireAuthorization()
             .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
             .WithName("CriarAdministrador")
             .WithSummary("Criar novo administrador")
             .WithDescription("Cria um novo administrador no sistema");
    }

    private static string GerarTokenJwt(Administrador administrador, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfig();
        configuration.GetSection("Jwt").Bind(jwtConfig);
        
        if(string.IsNullOrEmpty(jwtConfig.Key)) 
            return string.Empty;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim("Email", administrador.Email),
            new Claim("Perfil", administrador.Perfil),
            new Claim(ClaimTypes.Role, administrador.Perfil),
            new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()),
        };
        
        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(jwtConfig.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginDTO loginDTO, 
        IAdministradorServico administradorServico,
        IConfiguration configuration)
    {
        try
        {
            if (loginDTO == null || string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Senha))
                return Results.BadRequest("Email e senha são obrigatórios");

            var adm = await administradorServico.Login(loginDTO);
            if(adm != null)
            {
                string token = GerarTokenJwt(adm, configuration);
                return Results.Ok(new AdministradorLogado
                {
                    Email = adm.Email,
                    Perfil = adm.Perfil,
                    Token = token
                });
            }
            else
                return Results.Unauthorized("Credenciais inválidas");
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro interno do servidor");
        }
    }

    private static async Task<IResult> ListarAdministradores(
        [FromQuery] int? pagina, 
        IAdministradorServico administradorServico)
    {
        try
        {
            var adms = new List<AdministradorModelView>();
            var administradores = await administradorServico.Todos(pagina);
            
            foreach(var adm in administradores)
            {
                adms.Add(new AdministradorModelView{
                    Id = adm.Id,
                    Email = adm.Email,
                    Perfil = adm.Perfil
                });
            }
            
            return Results.Ok(adms);
        }
        catch
        {
            return Results.Problem("Erro ao buscar administradores");
        }
    }

    private static async Task<IResult> BuscarAdministradorPorId(
        [FromRoute] int id, 
        IAdministradorServico administradorServico)
    {
        try
        {
            var administrador = await administradorServico.BuscaPorId(id);
            if(administrador == null) 
                return Results.NotFound();
                
            return Results.Ok(new AdministradorModelView{
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
        }
        catch
        {
            return Results.Problem("Erro ao buscar administrador");
        }
    }

    private static async Task<IResult> CriarAdministrador(
        [FromBody] AdministradorDTO administradorDTO, 
        IAdministradorServico administradorServico)
    {
        try
        {
            var validacao = new ErrosDeValidacao{
                Mensagens = new List<string>()
            };

            if(string.IsNullOrEmpty(administradorDTO.Email))
                validacao.Mensagens.Add("Email não pode ser vazio");
            if(string.IsNullOrEmpty(administradorDTO.Senha))
                validacao.Mensagens.Add("Senha não pode ser vazia");
            if(administradorDTO.Perfil == null)
                validacao.Mensagens.Add("Perfil não pode ser vazio");

            if(validacao.Mensagens.Count > 0)
                return Results.BadRequest(validacao);
            
            var administrador = new Administrador{
                Email = administradorDTO.Email,
                Senha = administradorDTO.Senha,
                Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
            };

            await administradorServico.Incluir(administrador);

            return Results.Created($"/administradores/{administrador.Id}", new AdministradorModelView{
                Id = administrador.Id,
                Email = administrador.Email,
                Perfil = administrador.Perfil
            });
        }
        catch (Exception ex)
        {
            return Results.Problem("Erro ao criar administrador");
        }
    }
}
