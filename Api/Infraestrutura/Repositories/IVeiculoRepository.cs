using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;

namespace MinimalApi.Infraestrutura.Repositories;

public interface IVeiculoRepository : IRepository<Veiculo>
{
    Task<IEnumerable<Veiculo>> GetByMarcaAsync(string marca);
    Task<IEnumerable<Veiculo>> GetByAnoAsync(int ano);
    Task<IEnumerable<Veiculo>> GetByNomeAsync(string nome);
    Task<IEnumerable<Veiculo>> GetFilteredAsync(string? nome, string? marca, int? ano);
    Task<PaginatedResult<Veiculo>> GetPaginatedWithFiltersAsync(VeiculoFiltroDTO filtro);
    Task<int> GetTotalCountWithFiltersAsync(VeiculoFiltroDTO filtro);
}
