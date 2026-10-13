using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public interface ITipoResiduoRepository
    {
        IEnumerable<TipoResiduoModel> GetAll();
        TipoResiduoModel? GetById(int id);
        TipoResiduoModel? GetByNome(string nome);
        void Add(TipoResiduoModel tipoResiduo);
    }
}
