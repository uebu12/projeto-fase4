using AutoMapper;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.EcoColeta.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ColetaController : ControllerBase
    {
        private readonly IColetaService _service;
        private readonly IMapper _mapper;

        public ColetaController(IColetaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista coletas registradas com paginação e filtros opcionais
        /// por tipo de resíduo, ponto de coleta e intervalo de datas —
        /// otimizando a consulta para grandes volumes de dados.
        /// </summary>
        [HttpGet]
        public ActionResult<PaginacaoViewModel<ColetaViewModel>> Get(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10,
            [FromQuery] int? tipoResiduoId = null,
            [FromQuery] int? pontoColetaId = null,
            [FromQuery] DateTime? dataInicio = null,
            [FromQuery] DateTime? dataFim = null)
        {
            var (coletas, total) = _service.ListarColetas(
                pagina, tamanho, tipoResiduoId, pontoColetaId, dataInicio, dataFim);

            return Ok(new PaginacaoViewModel<ColetaViewModel>
            {
                Itens = _mapper.Map<IEnumerable<ColetaViewModel>>(coletas),
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalItens = total
            });
        }

        /// <summary>
        /// Relatório agregado de volume coletado, agrupado por tipo de
        /// resíduo e por ponto de coleta, com filtro de período.
        /// </summary>
        [HttpGet("relatorio-volume")]
        [Authorize(Roles = "analista,gestor")]
        public ActionResult<RelatorioVolumeViewModel> GetRelatorioVolume(
            [FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
        {
            return Ok(_service.GerarRelatorioVolume(dataInicio, dataFim));
        }

        /// <summary>
        /// Registra uma nova coleta. Atualiza automaticamente o volume
        /// acumulado do ponto de coleta e dispara a verificação de
        /// alerta de capacidade, retornando o alerta gerado (se houver)
        /// junto da coleta criada.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "operador,gestor")]
        public ActionResult Post([FromBody] ColetaCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var coleta = _mapper.Map<ColetaModel>(viewModel);
            var criada = _service.Registrar(coleta, out var alertaGerado);

            var resposta = new
            {
                coleta = _mapper.Map<ColetaViewModel>(criada),
                alertaGerado = alertaGerado != null
                    ? new
                    {
                        alertaGerado.AlertaId,
                        alertaGerado.Mensagem,
                        alertaGerado.PercentualOcupacao
                    }
                    : null
            };

            return CreatedAtAction(nameof(Get), null, resposta);
        }
    }
}
