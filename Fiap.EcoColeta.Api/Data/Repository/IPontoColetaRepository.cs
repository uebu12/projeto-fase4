using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public interface IPontoColetaRepository
    {
        (IEnumerable<PontoColetaModel> Itens, int Total) GetAll(int pagina, int tamanho);
        IEnumerable<PontoColetaModel> GetCriticos(decimal percentualLimite);
        PontoColetaModel? GetById(int id);
        void Add(PontoColetaModel pontoColeta);
        void Update(PontoColetaModel pontoColeta);
    }
}
