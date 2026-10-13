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
    public class PontoColetaController : ControllerBase
    {
        private readonly IPontoColetaService _service;
        private readonly IMapper _mapper;

        public PontoColetaController(IPontoColetaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista os pontos de coleta de forma paginada, já com o
        /// percentual de ocupação calculado para cada item.
        /// </summary>
        [HttpGet]
        public ActionResult<PaginacaoViewModel<PontoColetaViewModel>> Get(
            [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            var (pontos, total) = _service.ListarPontos(pagina, tamanho);
            var viewModels = MapearComPercentual(pontos);

            return Ok(new PaginacaoViewModel<PontoColetaViewModel>
            {
                Itens = viewModels,
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalItens = total
            });
        }

        /// <summary>
        /// Lista apenas os pontos de coleta em situação crítica de
        /// ocupação (percentual >= limite informado, padrão 80%).
        /// Esse endpoint concentra o valor de negócio do tema ESG,
        /// permitindo priorizar rotas de coleta.
        /// </summary>
        [HttpGet("criticos")]
        [Authorize(Roles = "analista,gestor")]
        public ActionResult<IEnumerable<PontoColetaViewModel>> GetCriticos([FromQuery] decimal percentual = 80)
        {
            var pontos = _service.ListarCriticos(percentual);
            return Ok(MapearComPercentual(pontos));
        }

        [HttpGet("{id}")]
        public ActionResult<PontoColetaViewModel> Get(int id)
        {
            var ponto = _service.ObterPorId(id);
            return Ok(MapearComPercentual(new[] { ponto }).First());
        }

        [HttpPost]
        [Authorize(Roles = "gestor")]
        public ActionResult<PontoColetaViewModel> Post([FromBody] PontoColetaCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ponto = _mapper.Map<PontoColetaModel>(viewModel);
            var criado = _service.Criar(ponto);

            return CreatedAtAction(nameof(Get), new { id = criado.PontoColetaId },
                MapearComPercentual(new[] { criado }).First());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "gestor")]
        public ActionResult<PontoColetaViewModel> Put(int id, [FromBody] PontoColetaUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dados = _mapper.Map<PontoColetaModel>(viewModel);
            var atualizado = _service.Atualizar(id, dados);

            return Ok(MapearComPercentual(new[] { atualizado }).First());
        }

        private IEnumerable<PontoColetaViewModel> MapearComPercentual(IEnumerable<PontoColetaModel> pontos)
        {
            return pontos.Select(p =>
            {
                var vm = _mapper.Map<PontoColetaViewModel>(p);
                vm.PercentualOcupacao = _service.CalcularPercentualOcupacao(p);
                return vm;
            });
        }
    }
}
