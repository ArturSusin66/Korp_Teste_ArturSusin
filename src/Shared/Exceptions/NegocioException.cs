namespace Korp.Shared.Exceptions;

/// <summary>
/// Exceção base para erros de lógica de negócio
/// Mapeia para HTTP 400 Bad Request
/// </summary>
public class NegocioException : Exception
{
    public NegocioException(string mensagem) : base(mensagem) { }
    public NegocioException(string mensagem, Exception innerException) 
        : base(mensagem, innerException) { }
}
