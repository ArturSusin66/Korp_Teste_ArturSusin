using FluentAssertions;
using Korp.Faturamento.Domain.Entities;
using Korp.Faturamento.Domain.Enums;
using Korp.Shared.Exceptions;
using Xunit;

namespace Korp.Faturamento.Tests.Unit.Domain;

public class NotaFiscalTests
{
    [Fact]
    public void CriarNotaFiscal_ComNumeroValido_Sucesso()
    {
        // Arrange & Act
        var nota = new NotaFiscal(1);

        // Assert
        nota.Numero.Should().Be(1);
        nota.Status.Should().Be(StatusNotaFiscal.Aberta);
        nota.Itens.Should().BeEmpty();
        nota.Total.Should().Be(0);
    }

    [Fact]
    public void CriarNotaFiscal_ComNumeroInvalido_LancaExcecao()
    {
        // Act & Assert
        Assert.Throws<NegocioException>(() => new NotaFiscal(0));
    }

    [Fact]
    public void AdicionarItem_ComDadosValidos_Sucesso()
    {
        // Arrange
        var nota = new NotaFiscal(1);

        // Act
        nota.AdicionarItem("P001", 2, 100.00m);

        // Assert
        nota.Itens.Should().HaveCount(1);
        nota.Itens[0].CodigoProduto.Should().Be("P001");
        nota.Itens[0].Quantidade.Should().Be(2);
        nota.Total.Should().Be(200.00m);
    }

    [Fact]
    public void AdicionarItem_EmNotaFechada_LancaExcecao()
    {
        // Arrange
        var nota = new NotaFiscal(1);
        nota.AdicionarItem("P001", 2, 100.00m);
        nota.Fechar();

        // Act & Assert
        Assert.Throws<NegocioException>(() => nota.AdicionarItem("P002", 1, 50.00m));
    }

    [Fact]
    public void Fechar_ComItens_Sucesso()
    {
        // Arrange
        var nota = new NotaFiscal(1);
        nota.AdicionarItem("P001", 2, 100.00m);

        // Act
        nota.Fechar();

        // Assert
        nota.Status.Should().Be(StatusNotaFiscal.Fechada);
        nota.DataFechamento.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Fechar_SemItens_LancaExcecao()
    {
        // Arrange
        var nota = new NotaFiscal(1);

        // Act & Assert
        Assert.Throws<NegocioException>(() => nota.Fechar());
    }

    [Fact]
    public void Fechar_NotaJaFechada_LancaExcecao()
    {
        // Arrange
        var nota = new NotaFiscal(1);
        nota.AdicionarItem("P001", 2, 100.00m);
        nota.Fechar();

        // Act & Assert
        Assert.Throws<NegocioException>(() => nota.Fechar());
    }

    [Fact]
    public void PodeSerImpresa_NotaAbertaComItens_RetornaTrue()
    {
        // Arrange
        var nota = new NotaFiscal(1);
        nota.AdicionarItem("P001", 2, 100.00m);

        // Act & Assert
        nota.PodeSerImpresa().Should().BeTrue();
    }

    [Fact]
    public void PodeSerImpresa_NotaVazia_RetornaFalse()
    {
        // Arrange
        var nota = new NotaFiscal(1);

        // Act & Assert
        nota.PodeSerImpresa().Should().BeFalse();
    }

    [Fact]
    public void PodeSerImpresa_NotaFechada_RetornaFalse()
    {
        // Arrange
        var nota = new NotaFiscal(1);
        nota.AdicionarItem("P001", 2, 100.00m);
        nota.Fechar();

        // Act & Assert
        nota.PodeSerImpresa().Should().BeFalse();
    }
}
