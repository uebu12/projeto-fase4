namespace Fiap.EcoColeta.Api.Models
{
    /// <summary>
    /// Registro de uma coleta de resíduos realizada em um PontoColeta.
    /// Cada coleta incrementa o volume acumulado do ponto e pode disparar
    /// a verificação automática de alertas de capacidade.
    /// </summary>
    public class ColetaModel
    {
        public int ColetaId { get; set; }

        public int PontoColetaId { get; set; }
        public PontoColetaModel? PontoColeta { get; set; }

        public int TipoResiduoId { get; set; }
        public TipoResiduoModel? TipoResiduo { get; set; }

        public decimal QuantidadeKg { get; set; }
        public DateTime DataColeta { get; set; } = DateTime.UtcNow;
        public string? Observacao { get; set; }
    }
}
