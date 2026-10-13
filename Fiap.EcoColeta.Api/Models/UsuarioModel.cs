namespace Fiap.EcoColeta.Api.Models
{
    /// <summary>
    /// Usuário para autenticação via JWT. Não possui endpoints de
    /// CRUD expostos (conforme orientação da atividade de evitar
    /// funcionalidades padrão de manutenção de usuários); a lista é
    /// mantida em memória pelo AuthService a título de simulação.
    /// </summary>
    public class UsuarioModel
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // operador | analista | gestor
    }
}
