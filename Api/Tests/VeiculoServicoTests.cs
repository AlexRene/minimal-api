using Microsoft.Extensions.Logging;
using Moq;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Repositories;
using Xunit;

namespace MinimalApi.Tests;

public class VeiculoServicoTests
{
    private readonly Mock<IVeiculoRepository> _mockRepository;
    private readonly VeiculoServico _servico;

    public VeiculoServicoTests()
    {
        _mockRepository = new Mock<IVeiculoRepository>();
        _servico = new VeiculoServico(_mockRepository.Object);
    }

    [Fact]
    public async Task BuscaPorId_QuandoIdExiste_DeveRetornarVeiculo()
    {
        // Arrange
        var id = 1;
        var veiculoEsperado = new Veiculo { Id = id, Nome = "Civic", Marca = "Honda", Ano = 2020 };
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(veiculoEsperado);

        // Act
        var resultado = await _servico.BuscaPorId(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(veiculoEsperado.Id, resultado.Id);
        Assert.Equal(veiculoEsperado.Nome, resultado.Nome);
        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task BuscaPorId_QuandoIdNaoExiste_DeveRetornarNull()
    {
        // Arrange
        var id = 999;
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Veiculo?)null);

        // Act
        var resultado = await _servico.BuscaPorId(id);

        // Assert
        Assert.Null(resultado);
        _mockRepository.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task Todos_QuandoPaginaValida_DeveRetornarLista()
    {
        // Arrange
        var pagina = 1;
        var veiculos = new List<Veiculo>
        {
            new Veiculo { Id = 1, Nome = "Civic", Marca = "Honda", Ano = 2020 },
            new Veiculo { Id = 2, Nome = "Corolla", Marca = "Toyota", Ano = 2021 }
        };
        _mockRepository.Setup(r => r.GetPagedAsync(pagina, 10)).ReturnsAsync(veiculos);

        // Act
        var resultado = await _servico.Todos(pagina);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
        _mockRepository.Verify(r => r.GetPagedAsync(pagina, 10), Times.Once);
    }

    [Fact]
    public async Task Todos_QuandoPaginaInvalida_DeveRetornarListaVazia()
    {
        // Arrange
        var pagina = -1;
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Veiculo>());

        // Act
        var resultado = await _servico.Todos(pagina);

        // Assert
        Assert.NotNull(resultado);
        Assert.Empty(resultado);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
