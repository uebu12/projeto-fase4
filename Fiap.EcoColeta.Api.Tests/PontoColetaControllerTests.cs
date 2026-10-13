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
    public class PontoColetaControllerTests
    {
        private readonly Mock<IPontoColetaService> _serviceMock;
        private readonly PontoColetaController _controller;

        public PontoColetaControllerTests()
        {
            _serviceMock = new Mock<IPontoColetaService>();
            _controller = new PontoColetaController(_serviceMock.Object, MapperFactory.Create());
        }

        [Fact]
        public void Get_ReturnsHttpStatusCode200_WithPaginatedList()
        {
            // Arrange
            var pontos = new List<PontoColetaModel>
            {
                new() { PontoColetaId = 1, Nome = "Ecoponto Vila Mariana", Endereco = "Rua A, 100",
                        Bairro = "Vila Mariana", CapacidadeMaximaKg = 500, VolumeAtualKg = 100, Ativo = true }
            };

            _serviceMock.Setup(s => s.ListarPontos(1, 10)).Returns((pontos, 1));
            _serviceMock.Setup(s => s.CalcularPercentualOcupacao(It.IsAny<PontoColetaModel>())).Returns(20m);

            // Act
            var result = _controller.Get(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);

            var body = Assert.IsType<PaginacaoViewModel<PontoColetaViewModel>>(okResult.Value);
            Assert.Equal(1, body.TotalItens);
        }
    }
}
