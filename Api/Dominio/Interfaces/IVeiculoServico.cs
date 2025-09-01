
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IVeiculoServico
{
    Task Apagar(Veiculo veiculo);
    Task Atualizar(Veiculo veiculo);
    Task<Veiculo?> BuscaPorId(int id);
    Task Incluir(Veiculo veiculo);
    Task<List<Veiculo>> Todos(int? pagina = 1, string? nome = null, string? marca = null);
    Task<PaginatedResult<Veiculo>> TodosComFiltrosAvancados(VeiculoFiltroDTO filtro);
    Task<IEnumerable<Veiculo>> BuscarPorMarca(string marca);
    Task<IEnumerable<Veiculo>> BuscarPorAno(int ano);
    Task<IEnumerable<Veiculo>> BuscarPorNome(string nome);
}