using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Dominio.Configuracoes;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Validators;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Infraestrutura.Repositories;
using MinimalApi.Endpoints;
using MinimalApi.Middleware;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        // Configuração JWT
        var jwtConfig = new JwtConfig();
        Configuration.GetSection("Jwt").Bind(jwtConfig);
        
        services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));
        services.Configure<CorsConfig>(Configuration.GetSection("Cors"));

        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConfig.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<AdministradorDTOValidator>();

        // Health Checks
        services.AddHealthChecks()
                .AddDbContextCheck<DbContexto>("Database");

        // Registro de Repositories
        services.AddScoped<IAdministradorRepository, AdministradorRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();

        // Registro de serviços
        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();
        services.AddScoped<ISenhaServico, SenhaServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Minimal API - Sistema de Gestão de Veículos",
                Version = "v1",
                Description = "API para gerenciamento de veículos e administradores com autenticação JWT",
                Contact = new OpenApiContact
                {
                    Name = "Equipe de Desenvolvimento",
                    Email = "dev@minimalapi.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            // Adicionar tags organizadas
            options.TagActionsBy(api => new[] { api.GroupName ?? api.RelativePath?.Split('/')[1] ?? "Default" });
            options.DocInclusionPredicate((name, api) => true);
        });

        services.AddDbContext<DbContexto>(options => {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
            );
        });

        // Configuração CORS mais segura
        var corsConfig = new CorsConfig();
        Configuration.GetSection("Cors").Bind(corsConfig);
        
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.WithOrigins(corsConfig.AllowedOrigins)
                        .WithMethods(corsConfig.AllowedMethods)
                        .WithHeaders(corsConfig.AllowedHeaders)
                        .AllowCredentials();
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        logger.LogInformation("Configurando aplicação para ambiente: {Environment}", env.EnvironmentName);

        // Middleware de tratamento global de exceções
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Minimal API - Documentação";
            options.DefaultModelsExpandDepth(2);
            options.DefaultModelExpandDepth(2);
        });

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints(endpoints => {
            // Health Check endpoint
            endpoints.MapHealthChecks("/health")
                   .AllowAnonymous()
                   .WithName("HealthCheck")
                   .WithTags("Monitoramento");

            // Mapear endpoints organizados
            endpoints.MapHomeEndpoints();
            endpoints.MapAdministradorEndpoints(Configuration);
            endpoints.MapVeiculoEndpoints();
            endpoints.MapEstatisticasEndpoints();
        });

        logger.LogInformation("Aplicação configurada com sucesso");
    }
}