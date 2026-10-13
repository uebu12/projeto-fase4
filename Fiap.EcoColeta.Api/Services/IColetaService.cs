using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.ViewModels;

namespace Fiap.EcoColeta.Api.Services
{
    public interface IColetaService
    {
        ColetaModel Registrar(ColetaModel coleta, out AlertaModel? alertaGerado);

        (IEnumerable<ColetaModel> Itens, int Total) ListarColetas(
            int pagina, int tamanho, int? tipoResiduoId, int? pontoColetaId,
            DateTime? dataInicio, DateTime? dataFim);

        RelatorioVolumeViewModel GerarRelatorioVolume(DateTime? dataInicio, DateTime? dataFim);
    }
}
