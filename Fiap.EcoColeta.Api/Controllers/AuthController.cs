using Fiap.EcoColeta.Api.Services;
using Fiap.EcoColeta.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fiap.EcoColeta.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT válido por 2 horas,
        /// que deve ser enviado no header Authorization (Bearer) das
        /// requisições subsequentes a endpoints protegidos.
        /// </summary>
        [HttpPost("login")]
        public ActionResult<LoginResponseViewModel> Login([FromBody] LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = _authService.Authenticate(viewModel.Username, viewModel.Password);
            if (usuario == null)
                return Unauthorized(new { erro = "Usuário ou senha inválidos." });

            var expiracao = DateTime.UtcNow.AddHours(2);
            var token = GerarToken(usuario.Username, usuario.Role, expiracao);

            return Ok(new LoginResponseViewModel
            {
                Token = token,
                Username = usuario.Username,
                Role = usuario.Role,
                ExpiraEm = expiracao
            });
        }

        private string GerarToken(string username, string role, DateTime expiracao)
        {
            var chaveSecreta = _configuration["Jwt:Key"]!;
            var secret = Encoding.UTF8.GetBytes(chaveSecreta);

            var securityKey = new SymmetricSecurityKey(secret);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = expiracao,
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
