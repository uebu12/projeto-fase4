namespace Fiap.EcoColeta.Api.Models
{
    /// <summary>
    /// Alerta automático gerado quando o percentual de ocupação de um
    /// PontoColeta atinge ou supera o limite configurado (padrão 80%).
    /// </summary>
    public class AlertaModel
    {
        public int AlertaId { get; set; }

        public int PontoColetaId { get; set; }
        public PontoColetaModel? PontoColeta { get; set; }

        public DateTime DataAlerta { get; set; } = DateTime.UtcNow;
        public decimal PercentualOcupacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool Resolvido { get; set; } = false;
        public DateTime? DataResolucao { get; set; }
    }
}
