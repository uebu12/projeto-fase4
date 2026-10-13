namespace Fiap.EcoColeta.Api.ViewModels
{
    /// <summary>
    /// Agrega os principais indicadores ESG do programa de gestão de
    /// resíduos: volumes coletados, pontos em situação crítica e
    /// estimativa de impacto ambiental positivo gerado pela reciclagem.
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalPontosColeta { get; set; }
        public int TotalPontosAtivos { get; set; }
        public int TotalPontosCriticos { get; set; }
        public int TotalColetasRegistradas { get; set; }
        public decimal VolumeTotalColetadoKg { get; set; }
        public Dictionary<string, decimal> VolumePorTipoResiduo { get; set; } = new();
        public int TotalAlertasAtivos { get; set; }
        public decimal ImpactoCO2EvitadoKg { get; set; }
    }

    public class ImpactoAmbientalViewModel
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal VolumeTotalRecicladoKg { get; set; }
        public decimal CO2EvitadoKg { get; set; }
        public decimal EquivalenteArvoresPreservadas { get; set; }
        public Dictionary<string, decimal> DetalhePorTipoResiduo { get; set; } = new();
        public string Observacao { get; set; } =
            "Estimativas calculadas com base em fatores médios de impacto por tipo de " +
            "resíduo reciclado. Valores ilustrativos para fins de acompanhamento ESG.";
    }
}
