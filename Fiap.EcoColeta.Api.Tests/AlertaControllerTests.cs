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
    public class AlertaControllerTests
    {
        private readonly Mock<IAlertaService> _serviceMock;
        private readonly AlertaController _controller;

        public AlertaControllerTests()
        {
            _serviceMock = new Mock<IAlertaService>();
            _controller = new AlertaController(_serviceMock.Object, MapperFactory.Create());
        }

        [Fact]
        public void Get_ReturnsHttpStatusCode200_WithPaginatedAlertas()
        {
            // Arrange
            var pontoColeta = new PontoColetaModel { PontoColetaId = 1, Nome = "Ecoponto Central" };
            var alertas = new List<AlertaModel>
            {
                new()
                {
                    AlertaId = 1, PontoColetaId = 1, PontoColeta = pontoColeta,
                    PercentualOcupacao = 85m, Mensagem = "Capacidade crítica", Resolvido = false
                }
            };

            _serviceMock.Setup(s => s.ListarAlertas(1, 10, null)).Returns((alertas, 1));

            // Act
            var result = _controller.Get(1, 10, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<PaginacaoViewModel<AlertaViewModel>>(okResult.Value);
            Assert.Single(body.Itens);
        }

        [Fact]
        public void VerificarTodos_ReturnsHttpStatusCode200()
        {
            // Arrange
            _serviceMock.Setup(s => s.VerificarTodosOsPontos(80m)).Returns(new List<AlertaModel>());

            // Act
            var result = _controller.VerificarTodos(80m);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
