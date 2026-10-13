using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public interface IColetaRepository
    {
        (IEnumerable<ColetaModel> Itens, int Total) GetAll(
            int pagina, int tamanho, int? tipoResiduoId, int? pontoColetaId,
            DateTime? dataInicio, DateTime? dataFim);

        void Add(ColetaModel coleta);

        Dictionary<string, decimal> GetVolumePorTipoResiduo(DateTime? dataInicio, DateTime? dataFim);
        Dictionary<string, decimal> GetVolumePorPontoColeta(DateTime? dataInicio, DateTime? dataFim);
        decimal GetVolumeTotal(DateTime? dataInicio, DateTime? dataFim);
        int GetTotalRegistros();
    }
}
