using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public interface IPontoColetaService
    {
        (IEnumerable<PontoColetaModel> Itens, int Total) ListarPontos(int pagina, int tamanho);
        IEnumerable<PontoColetaModel> ListarCriticos(decimal percentualLimite);
        PontoColetaModel ObterPorId(int id);
        PontoColetaModel Criar(PontoColetaModel pontoColeta);
        PontoColetaModel Atualizar(int id, PontoColetaModel dadosAtualizados);
        decimal CalcularPercentualOcupacao(PontoColetaModel pontoColeta);
    }
}
