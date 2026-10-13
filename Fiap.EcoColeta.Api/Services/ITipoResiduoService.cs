using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public interface ITipoResiduoService
    {
        IEnumerable<TipoResiduoModel> ListarTipos();
        TipoResiduoModel ObterPorId(int id);
        TipoResiduoModel Criar(TipoResiduoModel tipoResiduo);
    }
}
