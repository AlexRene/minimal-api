using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Infraestrutura.Repositories;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly IVeiculoRepository _repository;

    public VeiculoServico(IVeiculoRepository repository)
    {
        _repository = repository;
    }

    public async Task Apagar(Veiculo veiculo)
    {
        await _repository.DeleteAsync(veiculo);
    }

    public async Task Atualizar(Veiculo veiculo)
    {
        await _repository.UpdateAsync(veiculo);
    }

    public async Task<Veiculo?> BuscaPorId(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task Incluir(Veiculo veiculo)
    {
        await _repository.AddAsync(veiculo);
    }

    public async Task<List<Veiculo>> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        try
        {
            // Se há filtros, use o método filtrado
            if (!string.IsNullOrEmpty(nome) || !string.IsNullOrEmpty(marca))
            {
                var veiculos = await _repository.GetFilteredAsync(nome, marca, null);
                return veiculos.ToList();
            }

            // Caso contrário, use paginação
            if (pagina.HasValue && pagina > 0)
            {
                var veiculos = await _repository.GetPagedAsync(pagina.Value, 10);
                return veiculos.ToList();
            }

            var todos = await _repository.GetAllAsync();
            return todos.ToList();
        }
        catch
        {
            return new List<Veiculo>();
        }
    }

    public async Task<PaginatedResult<Veiculo>> TodosComFiltrosAvancados(VeiculoFiltroDTO filtro)
    {
        try
        {
            return await _repository.GetPaginatedWithFiltersAsync(filtro);
        }
        catch
        {
            return new PaginatedResult<Veiculo>
            {
                Data = new List<Veiculo>(),
                Metadata = new PaginationMetadata
                {
                    CurrentPage = filtro.Pagina ?? 1,
                    TotalPages = 0,
                    PageSize = filtro.TamanhoPagina ?? 10,
                    TotalCount = 0
                }
            };
        }
    }

    public async Task<IEnumerable<Veiculo>> BuscarPorMarca(string marca)
    {
        return await _repository.GetByMarcaAsync(marca);
    }

    public async Task<IEnumerable<Veiculo>> BuscarPorAno(int ano)
    {
        return await _repository.GetByAnoAsync(ano);
    }

    public async Task<IEnumerable<Veiculo>> BuscarPorNome(string nome)
    {
        return await _repository.GetByNomeAsync(nome);
    }
}