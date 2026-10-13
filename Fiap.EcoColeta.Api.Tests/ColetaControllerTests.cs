using Fiap.EcoColeta.Api.Controllers;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.Tests.Helpers;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fiap.EcoColeta.Api.Tests
{
    public class ColetaControllerTests
    {
        private readonly Mock<IColetaService> _serviceMock;
        private readonly ColetaController _controller;

        public ColetaControllerTests()
        {
            _serviceMock = new Mock<IColetaService>();
            _controller = new ColetaController(_serviceMock.Object, MapperFactory.Create());
        }

        [Fact]
        public void Get_ReturnsHttpStatusCode200_WithPaginatedColetas()
        {
            // Arrange
            var pontoColeta = new PontoColetaModel { PontoColetaId = 1, Nome = "Ecoponto Central" };
            var tipoResiduo = new TipoResiduoModel { TipoResiduoId = 1, Nome = "Plástico" };

            var coletas = new List<ColetaModel>
            {
                new()
                {
                    ColetaId = 1, PontoColetaId = 1, PontoColeta = pontoColeta,
                    TipoResiduoId = 1, TipoResiduo = tipoResiduo,
                    QuantidadeKg = 25.5m, DataColeta = DateTime.UtcNow
                }
            };

            _serviceMock
                .Setup(s => s.ListarColetas(1, 10, null, null, null, null))
                .Returns((coletas, 1));

            // Act
            var result = _controller.Get(1, 10, null, null, null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<PaginacaoViewModel<ColetaViewModel>>(okResult.Value);
            Assert.Single(body.Itens);
        }

        [Fact]
        public void GetRelatorioVolume_ReturnsHttpStatusCode200()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GerarRelatorioVolume(null, null))
                .Returns(new RelatorioVolumeViewModel
                {
                    VolumeTotalKg = 100m,
                    VolumePorTipoResiduo = new Dictionary<string, decimal> { { "Plástico", 100m } }
                });

            // Act
            var result = _controller.GetRelatorioVolume(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
