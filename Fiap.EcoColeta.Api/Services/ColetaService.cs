using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.Exceptions;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.ViewModels;

namespace Fiap.EcoColeta.Api.Services
{
    /// <summary>
    /// Orquestra o registro de novas coletas: valida o ponto de destino,
    /// atualiza o volume acumulado e aciona a verificação automática de
    /// alertas (IAlertaService), além de fornecer relatórios agregados
    /// de volume coletado por tipo de resíduo e por ponto de coleta.
    /// </summary>
    public class ColetaService : IColetaService
    {
        private readonly IColetaRepository _coletaRepository;
        private readonly IPontoColetaRepository _pontoColetaRepository;
        private readonly ITipoResiduoRepository _tipoResiduoRepository;
        private readonly IAlertaService _alertaService;

        public ColetaService(
            IColetaRepository coletaRepository,
            IPontoColetaRepository pontoColetaRepository,
            ITipoResiduoRepository tipoResiduoRepository,
            IAlertaService alertaService)
        {
            _coletaRepository = coletaRepository;
            _pontoColetaRepository = pontoColetaRepository;
            _tipoResiduoRepository = tipoResiduoRepository;
            _alertaService = alertaService;
        }

        public ColetaModel Registrar(ColetaModel coleta, out AlertaModel? alertaGerado)
        {
            var ponto = _pontoColetaRepository.GetById(coleta.PontoColetaId);
            if (ponto == null)
                throw new RecursoNaoEncontradoException(
                    $"Ponto de coleta com id {coleta.PontoColetaId} não encontrado.");

            if (!ponto.Ativo)
                throw new NegocioException(
                    $"O ponto de coleta '{ponto.Nome}' está inativo e não pode receber novas coletas.");

            var tipoResiduo = _tipoResiduoRepository.GetById(coleta.TipoResiduoId);
            if (tipoResiduo == null)
                throw new RecursoNaoEncontradoException(
                    $"Tipo de resíduo com id {coleta.TipoResiduoId} não encontrado.");

            coleta.DataColeta = DateTime.UtcNow;
            _coletaRepository.Add(coleta);

            // Atualiza o volume acumulado do ponto de coleta
            ponto.VolumeAtualKg += coleta.QuantidadeKg;
            _pontoColetaRepository.Update(ponto);

            // Verifica automaticamente se o novo volume exige a geração de um alerta
            alertaGerado = _alertaService.VerificarPonto(ponto);

            coleta.PontoColeta = ponto;
            coleta.TipoResiduo = tipoResiduo;
            return coleta;
        }

        public (IEnumerable<ColetaModel> Itens, int Total) ListarColetas(
            int pagina, int tamanho, int? tipoResiduoId, int? pontoColetaId,
            DateTime? dataInicio, DateTime? dataFim)
        {
            if (pagina < 1) pagina = 1;
            if (tamanho < 1 || tamanho > 100) tamanho = 10;

            return _coletaRepository.GetAll(pagina, tamanho, tipoResiduoId, pontoColetaId, dataInicio, dataFim);
        }

        public RelatorioVolumeViewModel GerarRelatorioVolume(DateTime? dataInicio, DateTime? dataFim)
        {
            return new RelatorioVolumeViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                VolumeTotalKg = _coletaRepository.GetVolumeTotal(dataInicio, dataFim),
                VolumePorTipoResiduo = _coletaRepository.GetVolumePorTipoResiduo(dataInicio, dataFim),
                VolumePorPontoColeta = _coletaRepository.GetVolumePorPontoColeta(dataInicio, dataFim)
            };
        }
    }
}
