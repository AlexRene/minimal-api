using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Infraestrutura.Repositories;

public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
{
    public AdministradorRepository(DbContexto context) : base(context)
    {
    }

    public async Task<Administrador?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Administrador?> GetByEmailAndPasswordAsync(string email, string hashedPassword)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Email == email && a.Senha == hashedPassword);
    }
}
