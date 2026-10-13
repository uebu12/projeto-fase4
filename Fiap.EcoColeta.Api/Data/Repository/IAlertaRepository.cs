using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Data.Repository
{
    public interface IAlertaRepository
    {
        (IEnumerable<AlertaModel> Itens, int Total) GetAll(int pagina, int tamanho, bool? somenteAtivos);
        AlertaModel? GetById(int id);
        void Add(AlertaModel alerta);
        void Update(AlertaModel alerta);
        bool ExisteAlertaAtivo(int pontoColetaId);
        int GetTotalAtivos();
    }
}
