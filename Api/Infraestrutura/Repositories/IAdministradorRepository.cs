using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Infraestrutura.Repositories;

public interface IAdministradorRepository : IRepository<Administrador>
{
    Task<Administrador?> GetByEmailAsync(string email);
    Task<Administrador?> GetByEmailAndPasswordAsync(string email, string hashedPassword);
}
