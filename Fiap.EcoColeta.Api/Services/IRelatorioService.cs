using Fiap.EcoColeta.Api.ViewModels;

namespace Fiap.EcoColeta.Api.Services
{
    public interface IRelatorioService
    {
        DashboardViewModel GerarDashboard();
        ImpactoAmbientalViewModel GerarImpactoAmbiental(DateTime? dataInicio, DateTime? dataFim);
    }
}
