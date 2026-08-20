namespace Korp.Shared.Exceptions;

/// <summary>
/// Exceção para falha de comunicação entre microsserviços
/// Mapeia para HTTP 503 Service Unavailable
/// </summary>
public class IntegracaoException : Exception
{
    public IntegracaoException(string mensagem) : base(mensagem) { }
    public IntegracaoException(string mensagem, Exception innerException) 
        : base(mensagem, innerException) { }
}
