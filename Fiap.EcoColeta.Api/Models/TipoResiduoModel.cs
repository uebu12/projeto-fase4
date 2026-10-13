namespace Fiap.EcoColeta.Api.Models
{
    /// <summary>
    /// Tipo de resíduo reciclável (Plástico, Papel, Vidro, Metal, Eletrônico, Orgânico...).
    /// FatorImpactoCO2 representa o kg de CO2 evitado para cada kg reciclado deste tipo,
    /// usado para compor os relatórios de impacto ambiental (ESG).
    /// </summary>
    public class TipoResiduoModel
    {
        public int TipoResiduoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal FatorImpactoCO2 { get; set; }
        public bool Reciclavel { get; set; } = true;

        public List<ColetaModel> Coletas { get; set; } = new();
    }
}
