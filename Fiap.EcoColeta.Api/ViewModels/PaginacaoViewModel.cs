namespace Fiap.EcoColeta.Api.ViewModels
{
    /// <summary>
    /// Envelope padrão de paginação usado por todos os endpoints de
    /// listagem da API, garantindo metadados consistentes (página atual,
    /// tamanho, total de itens e total de páginas) para o cliente.
    /// </summary>
    public class PaginacaoViewModel<T>
    {
        public IEnumerable<T> Itens { get; set; } = new List<T>();
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas => TamanhoPagina <= 0 ? 0 : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
        public bool TemPaginaAnterior => PaginaAtual > 1;
        public bool TemProximaPagina => PaginaAtual < TotalPaginas;
    }
}
