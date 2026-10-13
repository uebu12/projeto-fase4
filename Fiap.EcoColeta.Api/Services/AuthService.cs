using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    /// <summary>
    /// Serviço de autenticação. Mantém uma lista de usuários simulada em
    /// memória (mesmo padrão usado no material da FIAP), já que a
    /// atividade orienta a não investir em manutenção completa de
    /// usuários, e sim no domínio principal do projeto.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly List<UsuarioModel> _usuarios = new()
        {
            new UsuarioModel { UsuarioId = 1, Username = "operador01", Password = "pass123", Role = "operador" },
            new UsuarioModel { UsuarioId = 2, Username = "analista01", Password = "pass123", Role = "analista" },
            new UsuarioModel { UsuarioId = 3, Username = "gestor01",   Password = "pass123", Role = "gestor" },
        };

        public UsuarioModel? Authenticate(string username, string password)
        {
            return _usuarios.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);
        }
    }
}
