using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.Exceptions;
using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    /// <summary>
    /// Centraliza a regra de negócio de geração automática de alertas:
    /// sempre que um ponto de coleta atinge ou supera o percentual de
    /// ocupação configurado, um alerta é criado (caso não exista um
    /// alerta ativo equivalente), simulando o comportamento esperado de
    /// sensores IoT que monitoram o volume acumulado em tempo real.
    /// </summary>
    public class AlertaService : IAlertaService
    {
        private readonly IAlertaRepository _alertaRepository;
        private readonly IPontoColetaRepository _pontoColetaRepository;

        public AlertaService(IAlertaRepository alertaRepository, IPontoColetaRepository pontoColetaRepository)
        {
            _alertaRepository = alertaRepository;
            _pontoColetaRepository = pontoColetaRepository;
        }

        public AlertaModel? VerificarPonto(PontoColetaModel pontoColeta, decimal percentualLimite = 80m)
        {
            if (pontoColeta.CapacidadeMaximaKg <= 0) return null;

            var percentual = Math.Round((pontoColeta.VolumeAtualKg / pontoColeta.CapacidadeMaximaKg) * 100, 2);

            if (percentual < percentualLimite) return null;

            if (_alertaRepository.ExisteAlertaAtivo(pontoColeta.PontoColetaId)) return null;

            var alerta = new AlertaModel
            {
                PontoColetaId = pontoColeta.PontoColetaId,
                DataAlerta = DateTime.UtcNow,
                PercentualOcupacao = percentual,
                Mensagem = $"O ponto de coleta '{pontoColeta.Nome}' atingiu {percentual}% da capacidade " +
                           $"máxima ({pontoColeta.VolumeAtualKg}kg de {pontoColeta.CapacidadeMaximaKg}kg). " +
                           "Coleta recomendada.",
                Resolvido = false
            };

            _alertaRepository.Add(alerta);
            return alerta;
        }

        public List<AlertaModel> VerificarTodosOsPontos(decimal percentualLimite = 80m)
        {
            var (pontos, _) = _pontoColetaRepository.GetAll(1, int.MaxValue);
            var novosAlertas = new List<AlertaModel>();

            foreach (var ponto in pontos.Where(p => p.Ativo))
            {
                var alerta = VerificarPonto(ponto, percentualLimite);
                if (alerta != null) novosAlertas.Add(alerta);
            }

            return novosAlertas;
        }

        public (IEnumerable<AlertaModel> Itens, int Total) ListarAlertas(int pagina, int tamanho, bool? somenteAtivos)
        {
            if (pagina < 1) pagina = 1;
            if (tamanho < 1 || tamanho > 100) tamanho = 10;
            return _alertaRepository.GetAll(pagina, tamanho, somenteAtivos);
        }

        public AlertaModel Resolver(int alertaId)
        {
            var alerta = _alertaRepository.GetById(alertaId);
            if (alerta == null)
                throw new RecursoNaoEncontradoException($"Alerta com id {alertaId} não encontrado.");

            if (alerta.Resolvido)
                throw new NegocioException("Este alerta já foi resolvido anteriormente.");

            alerta.Resolvido = true;
            alerta.DataResolucao = DateTime.UtcNow;
            _alertaRepository.Update(alerta);

            return alerta;
        }
    }
}
