using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.Exceptions;
using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public class PontoColetaService : IPontoColetaService
    {
        private const decimal PercentualLimitePadrao = 80m;
        private readonly IPontoColetaRepository _repository;

        public PontoColetaService(IPontoColetaRepository repository)
        {
            _repository = repository;
        }

        public (IEnumerable<PontoColetaModel> Itens, int Total) ListarPontos(int pagina, int tamanho)
        {
            if (pagina < 1) pagina = 1;
            if (tamanho < 1 || tamanho > 100) tamanho = 10;
            return _repository.GetAll(pagina, tamanho);
        }

        public IEnumerable<PontoColetaModel> ListarCriticos(decimal percentualLimite)
        {
            var limite = percentualLimite <= 0 ? PercentualLimitePadrao : percentualLimite;
            return _repository.GetCriticos(limite);
        }

        public PontoColetaModel ObterPorId(int id)
        {
            var ponto = _repository.GetById(id);
            if (ponto == null)
                throw new RecursoNaoEncontradoException($"Ponto de coleta com id {id} não encontrado.");
            return ponto;
        }

        public PontoColetaModel Criar(PontoColetaModel pontoColeta)
        {
            pontoColeta.VolumeAtualKg = 0;
            pontoColeta.Ativo = true;
            pontoColeta.DataCadastro = DateTime.UtcNow;
            _repository.Add(pontoColeta);
            return pontoColeta;
        }

        public PontoColetaModel Atualizar(int id, PontoColetaModel dadosAtualizados)
        {
            var ponto = ObterPorId(id);

            if (dadosAtualizados.CapacidadeMaximaKg < ponto.VolumeAtualKg)
                throw new NegocioException(
                    "A nova capacidade máxima não pode ser menor que o volume atualmente acumulado no ponto.");

            ponto.Nome = dadosAtualizados.Nome;
            ponto.Endereco = dadosAtualizados.Endereco;
            ponto.Bairro = dadosAtualizados.Bairro;
            ponto.CapacidadeMaximaKg = dadosAtualizados.CapacidadeMaximaKg;
            ponto.Ativo = dadosAtualizados.Ativo;

            _repository.Update(ponto);
            return ponto;
        }

        public decimal CalcularPercentualOcupacao(PontoColetaModel pontoColeta)
        {
            if (pontoColeta.CapacidadeMaximaKg <= 0) return 0;
            return Math.Round((pontoColeta.VolumeAtualKg / pontoColeta.CapacidadeMaximaKg) * 100, 2);
        }
    }
}
