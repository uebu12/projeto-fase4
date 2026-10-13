using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.ViewModels;

namespace Fiap.EcoColeta.Api.Services
{
    /// <summary>
    /// Camada de inteligência ESG do projeto: combina dados de coletas,
    /// pontos de coleta e alertas para produzir indicadores de
    /// sustentabilidade — volume reciclado, pontos críticos e estimativa
    /// de impacto ambiental positivo (CO2 evitado / árvores preservadas).
    /// </summary>
    public class RelatorioService : IRelatorioService
    {
        // Fator ilustrativo: 1 árvore adulta absorve, em média, ~22kg de CO2 por ano.
        private const decimal KgCo2PorArvorePorAno = 22m;

        private readonly IColetaRepository _coletaRepository;
        private readonly IPontoColetaRepository _pontoColetaRepository;
        private readonly IAlertaRepository _alertaRepository;
        private readonly ITipoResiduoRepository _tipoResiduoRepository;

        public RelatorioService(
            IColetaRepository coletaRepository,
            IPontoColetaRepository pontoColetaRepository,
            IAlertaRepository alertaRepository,
            ITipoResiduoRepository tipoResiduoRepository)
        {
            _coletaRepository = coletaRepository;
            _pontoColetaRepository = pontoColetaRepository;
            _alertaRepository = alertaRepository;
            _tipoResiduoRepository = tipoResiduoRepository;
        }

        public DashboardViewModel GerarDashboard()
        {
            var (pontos, totalPontos) = _pontoColetaRepository.GetAll(1, int.MaxValue);
            var pontosAtivos = pontos.Count(p => p.Ativo);
            var pontosCriticos = _pontoColetaRepository.GetCriticos(80m).Count();

            var volumePorTipo = _coletaRepository.GetVolumePorTipoResiduo(null, null);
            var volumeTotal = _coletaRepository.GetVolumeTotal(null, null);

            var co2Evitado = CalcularCo2Evitado(volumePorTipo);

            return new DashboardViewModel
            {
                TotalPontosColeta = totalPontos,
                TotalPontosAtivos = pontosAtivos,
                TotalPontosCriticos = pontosCriticos,
                TotalColetasRegistradas = _coletaRepository.GetTotalRegistros(),
                VolumeTotalColetadoKg = volumeTotal,
                VolumePorTipoResiduo = volumePorTipo,
                TotalAlertasAtivos = _alertaRepository.GetTotalAtivos(),
                ImpactoCO2EvitadoKg = co2Evitado
            };
        }

        public ImpactoAmbientalViewModel GerarImpactoAmbiental(DateTime? dataInicio, DateTime? dataFim)
        {
            var volumePorTipo = _coletaRepository.GetVolumePorTipoResiduo(dataInicio, dataFim);
            var volumeTotal = _coletaRepository.GetVolumeTotal(dataInicio, dataFim);
            var co2Evitado = CalcularCo2Evitado(volumePorTipo);

            var detalhePorTipo = new Dictionary<string, decimal>();
            var tiposResiduo = _tipoResiduoRepository.GetAll().ToDictionary(t => t.Nome, t => t.FatorImpactoCO2);

            foreach (var (nomeTipo, volume) in volumePorTipo)
            {
                var fator = tiposResiduo.TryGetValue(nomeTipo, out var f) ? f : 0;
                detalhePorTipo[nomeTipo] = Math.Round(volume * fator, 2);
            }

            return new ImpactoAmbientalViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                VolumeTotalRecicladoKg = volumeTotal,
                CO2EvitadoKg = co2Evitado,
                EquivalenteArvoresPreservadas = Math.Round(co2Evitado / KgCo2PorArvorePorAno, 1),
                DetalhePorTipoResiduo = detalhePorTipo
            };
        }

        private decimal CalcularCo2Evitado(Dictionary<string, decimal> volumePorTipo)
        {
            var tiposResiduo = _tipoResiduoRepository.GetAll().ToDictionary(t => t.Nome, t => t.FatorImpactoCO2);

            decimal total = 0;
            foreach (var (nomeTipo, volume) in volumePorTipo)
            {
                if (tiposResiduo.TryGetValue(nomeTipo, out var fator))
                    total += volume * fator;
            }

            return Math.Round(total, 2);
        }
    }
}
