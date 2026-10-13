using AutoMapper;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.EcoColeta.Api.Controllers
{
    /// <summary>
    /// Entidade de apoio (tabela de domínio) usada pelas coletas.
    /// Mantida deliberadamente simples: a complexidade e o valor do
    /// projeto estão concentrados em PontoColeta, Coleta, Alerta e
    /// Relatorio, conforme orientação da atividade.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoResiduoController : ControllerBase
    {
        private readonly ITipoResiduoService _service;
        private readonly IMapper _mapper;

        public TipoResiduoController(ITipoResiduoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TipoResiduoViewModel>> Get()
        {
            var tipos = _service.ListarTipos();
            return Ok(_mapper.Map<IEnumerable<TipoResiduoViewModel>>(tipos));
        }

        [HttpGet("{id}")]
        public ActionResult<TipoResiduoViewModel> Get(int id)
        {
            var tipo = _service.ObterPorId(id);
            return Ok(_mapper.Map<TipoResiduoViewModel>(tipo));
        }

        [HttpPost]
        [Authorize(Roles = "gestor")]
        public ActionResult<TipoResiduoViewModel> Post([FromBody] TipoResiduoCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tipo = _mapper.Map<TipoResiduoModel>(viewModel);
            var criado = _service.Criar(tipo);

            return CreatedAtAction(nameof(Get), new { id = criado.TipoResiduoId },
                _mapper.Map<TipoResiduoViewModel>(criado));
        }
    }
}
