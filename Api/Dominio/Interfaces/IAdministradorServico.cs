
using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    Task<Administrador?> BuscaPorId(int id);
    Task<Administrador> Incluir(Administrador administrador);
    Task<Administrador?> Login(LoginDTO loginDTO);
    Task<List<Administrador>> Todos(int? pagina);
}