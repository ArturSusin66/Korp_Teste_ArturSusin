using FluentAssertions;
using Korp.Estoque.Domain.Entities;
using Korp.Shared.Exceptions;
using Xunit;

namespace Korp.Estoque.Tests.Unit.Domain;

public class ProdutoTests
{
    [Fact]
    public void CriarProduto_ComDadosValidos_Sucesso()
    {
        // Arrange & Act
        var produto = new Produto("P001", "Notebook", 10);

        // Assert
        produto.Codigo.Should().Be("P001");
        produto.Descricao.Should().Be("Notebook");
        produto.Saldo.Should().Be(10);
        produto.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarProduto_CodigoVazio_LancaExcecao()
    {
        // Arrange & Act & Assert
        Assert.Throws<NegocioException>(() => new Produto("", "Notebook", 10));
    }

    [Fact]
    public void CriarProduto_DescricaoVazia_LancaExcecao()
    {
        // Arrange & Act & Assert
        Assert.Throws<NegocioException>(() => new Produto("P001", "", 10));
    }

    [Fact]
    public void CriarProduto_SaldoNegativo_LancaExcecao()
    {
        // Arrange & Act & Assert
        Assert.Throws<NegocioException>(() => new Produto("P001", "Notebook", -5));
    }

    [Fact]
    public void ReducirSaldo_ComQuantidadeValida_Sucesso()
    {
        // Arrange
        var produto = new Produto("P001", "Notebook", 10);

        // Act
        produto.ReducirSaldo(3);

        // Assert
        produto.Saldo.Should().Be(7);
        produto.AtualizadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ReducirSaldo_ComQuantidadeInsuficiente_LancaExcecao()
    {
        // Arrange
        var produto = new Produto("P001", "Notebook", 5);

        // Act & Assert
        var excecao = Assert.Throws<NegocioException>(() => produto.ReducirSaldo(10));
        excecao.Message.Should().Contain("Saldo insuficiente");
    }

    [Fact]
    public void ReducirSaldo_ComQuantidadeZero_LancaExcecao()
    {
        // Arrange
        var produto = new Produto("P001", "Notebook", 10);

        // Act & Assert
        Assert.Throws<NegocioException>(() => produto.ReducirSaldo(0));
    }

    [Fact]
    public void AdicionarSaldo_ComQuantidadeValida_Sucesso()
    {
        // Arrange
        var produto = new Produto("P001", "Notebook", 5);

        // Act
        produto.AdicionarSaldo(3);

        // Assert
        produto.Saldo.Should().Be(8);
    }
}
