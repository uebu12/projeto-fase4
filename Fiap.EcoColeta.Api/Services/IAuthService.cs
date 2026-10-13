using Fiap.EcoColeta.Api.Models;

namespace Fiap.EcoColeta.Api.Services
{
    public interface IAuthService
    {
        UsuarioModel? Authenticate(string username, string password);
    }
}
