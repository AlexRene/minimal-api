using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;

namespace MinimalApi.Infraestrutura.Repositories;

public class VeiculoRepository : Repository<Veiculo>, IVeiculoRepository
{
    public VeiculoRepository(DbContexto context) : base(context)
    {
    }

    public async Task<IEnumerable<Veiculo>> GetByMarcaAsync(string marca)
    {
        return await _dbSet
            .Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{marca.ToLower()}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Veiculo>> GetByAnoAsync(int ano)
    {
        return await _dbSet
            .Where(v => v.Ano == ano)
            .ToListAsync();
    }

    public async Task<IEnumerable<Veiculo>> GetByNomeAsync(string nome)
    {
        return await _dbSet
            .Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"))
            .ToListAsync();
    }

    public async Task<IEnumerable<Veiculo>> GetFilteredAsync(string? nome, string? marca, int? ano)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome.ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            query = query.Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{marca.ToLower()}%"));
        }

        if (ano.HasValue)
        {
            query = query.Where(v => v.Ano == ano.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<PaginatedResult<Veiculo>> GetPaginatedWithFiltersAsync(VeiculoFiltroDTO filtro)
    {
        var query = BuildFilteredQuery(filtro);
        
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / filtro.TamanhoPagina!.Value);
        
        // Aplicar ordenação
        query = ApplyOrdering(query, filtro.OrdenarPor!, filtro.OrdenacaoAscendente);
        
        // Aplicar paginação
        var data = await query
            .Skip((filtro.Pagina!.Value - 1) * filtro.TamanhoPagina.Value)
            .Take(filtro.TamanhoPagina.Value)
            .ToListAsync();

        return new PaginatedResult<Veiculo>
        {
            Data = data,
            Metadata = new PaginationMetadata
            {
                CurrentPage = filtro.Pagina.Value,
                TotalPages = totalPages,
                PageSize = filtro.TamanhoPagina.Value,
                TotalCount = totalCount
            }
        };
    }

    public async Task<int> GetTotalCountWithFiltersAsync(VeiculoFiltroDTO filtro)
    {
        var query = BuildFilteredQuery(filtro);
        return await query.CountAsync();
    }

    private IQueryable<Veiculo> BuildFilteredQuery(VeiculoFiltroDTO filtro)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Nome))
        {
            query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{filtro.Nome.ToLower()}%"));
        }

        if (!string.IsNullOrEmpty(filtro.Marca))
        {
            query = query.Where(v => EF.Functions.Like(v.Marca.ToLower(), $"%{filtro.Marca.ToLower()}%"));
        }

        if (filtro.AnoMinimo.HasValue)
        {
            query = query.Where(v => v.Ano >= filtro.AnoMinimo.Value);
        }

        if (filtro.AnoMaximo.HasValue)
        {
            query = query.Where(v => v.Ano <= filtro.AnoMaximo.Value);
        }

        return query;
    }

    private IQueryable<Veiculo> ApplyOrdering(IQueryable<Veiculo> query, string ordenarPor, bool ascendente)
    {
        return ordenarPor.ToLower() switch
        {
            "nome" => ascendente ? query.OrderBy(v => v.Nome) : query.OrderByDescending(v => v.Nome),
            "marca" => ascendente ? query.OrderBy(v => v.Marca) : query.OrderByDescending(v => v.Marca),
            "ano" => ascendente ? query.OrderBy(v => v.Ano) : query.OrderByDescending(v => v.Ano),
            "id" => ascendente ? query.OrderBy(v => v.Id) : query.OrderByDescending(v => v.Id),
            _ => ascendente ? query.OrderBy(v => v.Nome) : query.OrderByDescending(v => v.Nome)
        };
    }
}
