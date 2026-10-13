namespace Fiap.EcoColeta.Api.Exceptions
{
    /// <summary>
    /// Exceção lançada quando uma regra de negócio é violada
    /// (ex.: capacidade do ponto de coleta excedida, recurso inexistente
    /// em uma operação de atualização). Capturada pelo ExceptionMiddleware
    /// e traduzida em uma resposta HTTP 400 estruturada.
    /// </summary>
    public class NegocioException : Exception
    {
        public NegocioException(string message) : base(message) { }
    }

    /// <summary>
    /// Exceção lançada quando um recurso solicitado não é encontrado,
    /// traduzida pelo ExceptionMiddleware em uma resposta HTTP 404.
    /// </summary>
    public class RecursoNaoEncontradoException : Exception
    {
        public RecursoNaoEncontradoException(string message) : base(message) { }
    }
}
