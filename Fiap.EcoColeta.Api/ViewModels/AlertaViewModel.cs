namespace Fiap.EcoColeta.Api.ViewModels
{
    public class AlertaViewModel
    {
        public int AlertaId { get; set; }
        public int PontoColetaId { get; set; }
        public string PontoColetaNome { get; set; } = string.Empty;
        public DateTime DataAlerta { get; set; }
        public decimal PercentualOcupacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool Resolvido { get; set; }
        public DateTime? DataResolucao { get; set; }
    }

    public class VerificacaoAlertaResultViewModel
    {
        public int PontosVerificados { get; set; }
        public int AlertasGerados { get; set; }
        public List<AlertaViewModel> NovosAlertas { get; set; } = new();
    }
}
