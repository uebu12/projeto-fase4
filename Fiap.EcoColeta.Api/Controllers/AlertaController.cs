using AutoMapper;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.EcoColeta.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlertaController : ControllerBase
    {
        private readonly IAlertaService _service;
        private readonly IMapper _mapper;

        public AlertaController(IAlertaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista os alertas de capacidade gerados, com paginação e
        /// filtro opcional para mostrar apenas os ainda não resolvidos.
        /// </summary>
        [HttpGet]
        public ActionResult<PaginacaoViewModel<AlertaViewModel>> Get(
            [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10, [FromQuery] bool? somenteAtivos = null)
        {
            var (alertas, total) = _service.ListarAlertas(pagina, tamanho, somenteAtivos);

            return Ok(new PaginacaoViewModel<AlertaViewModel>
            {
                Itens = _mapper.Map<IEnumerable<AlertaViewModel>>(alertas),
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalItens = total
            });
        }

        /// <summary>
        /// Dispara manualmente a varredura de todos os pontos de coleta
        /// ativos, simulando o comportamento de sensores IoT que
        /// monitoram a ocupação em tempo real e geram alertas automáticos
        /// quando o limite configurado é atingido.
        /// </summary>
        [HttpPost("verificar")]
        [Authorize(Roles = "gestor")]
        public ActionResult<VerificacaoAlertaResultViewModel> VerificarTodos([FromQuery] decimal percentualLimite = 80)
        {
            var novosAlertas = _service.VerificarTodosOsPontos(percentualLimite);

            return Ok(new VerificacaoAlertaResultViewModel
            {
                PontosVerificados = novosAlertas.Count,
                AlertasGerados = novosAlertas.Count,
                NovosAlertas = _mapper.Map<List<AlertaViewModel>>(novosAlertas)
            });
        }

        /// <summary>
        /// Marca um alerta como resolvido, registrando a data de resolução.
        /// </summary>
        [HttpPut("{id}/resolver")]
        [Authorize(Roles = "gestor")]
        public ActionResult<AlertaViewModel> Resolver(int id)
        {
            var alerta = _service.Resolver(id);
            return Ok(_mapper.Map<AlertaViewModel>(alerta));
        }
    }
}
