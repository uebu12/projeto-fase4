using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.EcoColeta.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelos indicadores ESG agregados do
    /// programa de gestão de resíduos: visão geral (dashboard) e
    /// estimativa detalhada de impacto ambiental positivo.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "analista,gestor")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _service;

        public RelatorioController(IRelatorioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Visão geral do programa: total de pontos, pontos críticos,
        /// volume coletado por tipo de resíduo, alertas ativos e
        /// estimativa de CO2 evitado.
        /// </summary>
        [HttpGet("dashboard")]
        public ActionResult<DashboardViewModel> GetDashboard()
        {
            return Ok(_service.GerarDashboard());
        }

        /// <summary>
        /// Estimativa detalhada de impacto ambiental (CO2 evitado e
        /// equivalente em árvores preservadas) com filtro de período,
        /// detalhado por tipo de resíduo reciclado.
        /// </summary>
        [HttpGet("impacto-ambiental")]
        public ActionResult<ImpactoAmbientalViewModel> GetImpactoAmbiental(
            [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            return Ok(_service.GerarImpactoAmbiental(dataInicio, dataFim));
        }
    }
}
