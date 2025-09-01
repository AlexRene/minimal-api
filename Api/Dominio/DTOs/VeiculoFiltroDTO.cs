namespace MinimalApi.DTOs;

public class VeiculoFiltroDTO
{
    public string? Nome { get; set; }
    public string? Marca { get; set; }
    public int? AnoMinimo { get; set; }
    public int? AnoMaximo { get; set; }
    public int? Pagina { get; set; } = 1;
    public int? TamanhoPagina { get; set; } = 10;
    public string? OrdenarPor { get; set; } = "Nome";
    public bool OrdenacaoAscendente { get; set; } = true;
}
