using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.Exceptions;
using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public class TipoResiduoService : ITipoResiduoService
    {
        private readonly ITipoResiduoRepository _repository;

        public TipoResiduoService(ITipoResiduoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<TipoResiduoModel> ListarTipos() => _repository.GetAll();

        public TipoResiduoModel ObterPorId(int id)
        {
            var tipo = _repository.GetById(id);
            if (tipo == null)
                throw new RecursoNaoEncontradoException($"Tipo de resíduo com id {id} não encontrado.");
            return tipo;
        }

        public TipoResiduoModel Criar(TipoResiduoModel tipoResiduo)
        {
            var existente = _repository.GetByNome(tipoResiduo.Nome);
            if (existente != null)
                throw new NegocioException($"Já existe um tipo de resíduo cadastrado com o nome '{tipoResiduo.Nome}'.");

            _repository.Add(tipoResiduo);
            return tipoResiduo;
        }
    }
}
