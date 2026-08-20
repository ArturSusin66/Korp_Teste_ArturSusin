using Korp.Shared.Exceptions;

namespace Korp.Estoque.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa um Produto no estoque
/// Contém lógica de negócio relacionada a produtos
/// </summary>
public class Produto
{
    // Propriedades
    public int Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int Saldo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }

    // Construtor privado para EF Core
    private Produto() { }

    // Construtor public para criar novo produto
    public Produto(string codigo, string descricao, int saldo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new NegocioException("Código do produto é obrigatório.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new NegocioException("Descrição do produto é obrigatória.");

        if (saldo < 0)
            throw new NegocioException("Saldo não pode ser negativo.");

        Codigo = codigo.Trim();
        Descricao = descricao.Trim();
        Saldo = saldo;
        CriadoEm = DateTime.UtcNow;
    }

    // Métodos de negócio
    public void ReducirSaldo(int quantidade)
    {
        if (quantidade <= 0)
            throw new NegocioException("Quantidade a reduzir deve ser maior que zero.");

        if (Saldo < quantidade)
            throw new NegocioException($"Saldo insuficiente. Disponível: {Saldo}, Solicitado: {quantidade}");

        Saldo -= quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void AdicionarSaldo(int quantidade)
    {
        if (quantidade <= 0)
            throw new NegocioException("Quantidade a adicionar deve ser maior que zero.");

        Saldo += quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }
}
