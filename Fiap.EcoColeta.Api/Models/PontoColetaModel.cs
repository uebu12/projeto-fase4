namespace Fiap.EcoColeta.Api.Models
{
    /// <summary>
    /// Ponto físico de coleta seletiva (ecoponto, container, cooperativa parceira).
    /// VolumeAtualKg é atualizado a cada nova coleta registrada e serve de base
    /// para o cálculo de ocupação e geração automática de alertas.
    /// </summary>
    public class PontoColetaModel
    {
        public int PontoColetaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public decimal CapacidadeMaximaKg { get; set; }
        public decimal VolumeAtualKg { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public List<ColetaModel> Coletas { get; set; } = new();
        public List<AlertaModel> Alertas { get; set; } = new();
    }
}
