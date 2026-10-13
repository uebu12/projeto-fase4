using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public interface IAlertaService
    {
        AlertaModel? VerificarPonto(PontoColetaModel pontoColeta, decimal percentualLimite = 80m);
        List<AlertaModel> VerificarTodosOsPontos(decimal percentualLimite = 80m);
        (IEnumerable<AlertaModel> Itens, int Total) ListarAlertas(int pagina, int tamanho, bool? somenteAtivos);
        AlertaModel Resolver(int alertaId);
    }
}
