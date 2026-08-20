namespace Korp.Shared.Exceptions;

/// <summary>
/// Exceção para validação de dados de entrada
/// Mapeia para HTTP 400 Bad Request
/// </summary>
public class ValidacaoException : Exception
{
    public ValidacaoException(string mensagem) : base(mensagem) { }
    public ValidacaoException(string mensagem, Exception innerException) 
        : base(mensagem, innerException) { }
}
