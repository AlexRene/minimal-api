using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Repositories;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly IAdministradorRepository _repository;
    private readonly ISenhaServico _senhaServico;

    public AdministradorServico(IAdministradorRepository repository, ISenhaServico senhaServico)
    {
        _repository = repository;
        _senhaServico = senhaServico;
    }

    public async Task<Administrador?> BuscaPorId(int id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Administrador> Incluir(Administrador administrador)
    {
        try
        {
            // Hash da senha antes de salvar
            administrador.Senha = _senhaServico.HashSenha(administrador.Senha);
            
            return await _repository.AddAsync(administrador);
        }
        catch
        {
            throw new InvalidOperationException("Erro ao incluir administrador");
        }
    }

    public async Task<Administrador?> Login(LoginDTO loginDTO)
    {
        try
        {
            var administrador = await _repository.GetByEmailAsync(loginDTO.Email);
            
            if (administrador == null)
                return null;

            // Verifica se a senha está correta usando hash
            if (_senhaServico.VerificarSenha(loginDTO.Senha, administrador.Senha))
                return administrador;

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Administrador>> Todos(int? pagina)
    {
        try
        {
            if (pagina.HasValue && pagina > 0)
            {
                var administradores = await _repository.GetPagedAsync(pagina.Value, 10);
                return administradores.ToList();
            }
            
            var todos = await _repository.GetAllAsync();
            return todos.ToList();
        }
        catch
        {
            return new List<Administrador>();
        }
    }
}