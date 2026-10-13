using Fiap.EcoColeta.Api.Controllers;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fiap.EcoColeta.Api.Tests
{
    public class TipoResiduoControllerTests
    {
        private readonly Mock<ITipoResiduoService> _serviceMock;
        private readonly TipoResiduoController _controller;

        public TipoResiduoControllerTests()
        {
            _serviceMock = new Mock<ITipoResiduoService>();
            _controller = new TipoResiduoController(_serviceMock.Object, MapperFactory.Create());
        }

        [Fact]
        public void Get_ReturnsHttpStatusCode200_WithListOfTipos()
        {
            // Arrange
            _serviceMock.Setup(s => s.ListarTipos()).Returns(new List<TipoResiduoModel>
            {
                new() { TipoResiduoId = 1, Nome = "Plástico", FatorImpactoCO2 = 1.5m, Reciclavel = true },
                new() { TipoResiduoId = 2, Nome = "Papel/Papelão", FatorImpactoCO2 = 0.9m, Reciclavel = true }
            });

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
