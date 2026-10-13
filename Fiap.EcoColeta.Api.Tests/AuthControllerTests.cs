using Fiap.EcoColeta.Api.Controllers;
using Fiap.EcoColeta.Api.Models;
using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Fiap.EcoColeta.Api.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["Jwt:Key"])
                .Returns("Chave_Secreta_De_Teste_Com_Tamanho_Suficiente_2025");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("EcoColetaApi.Tests");

            _controller = new AuthController(_authServiceMock.Object, _configurationMock.Object);
        }

        [Fact]
        public void Login_ReturnsHttpStatusCode200_WhenCredentialsAreValid()
        {
            // Arrange
            _authServiceMock
                .Setup(s => s.Authenticate("gestor01", "pass123"))
                .Returns(new UsuarioModel { UsuarioId = 1, Username = "gestor01", Password = "pass123", Role = "gestor" });

            var request = new LoginViewModel { Username = "gestor01", Password = "pass123" };

            // Act
            var result = _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            _authServiceMock
                .Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((UsuarioModel?)null);

            var request = new LoginViewModel { Username = "inexistente", Password = "errado" };

            // Act
            var result = _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}
