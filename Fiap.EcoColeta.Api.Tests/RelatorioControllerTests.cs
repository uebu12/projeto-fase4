using Fiap.EcoColeta.Api.Controllers;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fiap.EcoColeta.Api.Tests
{
    public class RelatorioControllerTests
    {
        private readonly Mock<IRelatorioService> _serviceMock;
        private readonly RelatorioController _controller;

        public RelatorioControllerTests()
        {
            _serviceMock = new Mock<IRelatorioService>();
            _controller = new RelatorioController(_serviceMock.Object);
        }

        [Fact]
        public void GetDashboard_ReturnsHttpStatusCode200()
        {
            // Arrange
            _serviceMock.Setup(s => s.GerarDashboard()).Returns(new DashboardViewModel
            {
                TotalPontosColeta = 5,
                TotalPontosAtivos = 5,
                TotalPontosCriticos = 1,
                TotalColetasRegistradas = 20,
                VolumeTotalColetadoKg = 350m,
                TotalAlertasAtivos = 1,
                ImpactoCO2EvitadoKg = 120m
            });

            // Act
            var result = _controller.GetDashboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void GetImpactoAmbiental_ReturnsHttpStatusCode200()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GerarImpactoAmbiental(null, null))
                .Returns(new ImpactoAmbientalViewModel
                {
                    VolumeTotalRecicladoKg = 350m,
                    CO2EvitadoKg = 120m,
                    EquivalenteArvoresPreservadas = 5.5m
                });

            // Act
            var result = _controller.GetImpactoAmbiental(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
