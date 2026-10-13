using System.ComponentModel.DataAnnotations;

namespace Fiap.EcoColeta.Api.ViewModels
{
    public class ColetaViewModel
    {
        public int ColetaId { get; set; }
        public int PontoColetaId { get; set; }
        public string PontoColetaNome { get; set; } = string.Empty;
        public int TipoResiduoId { get; set; }
        public string TipoResiduoNome { get; set; } = string.Empty;
        public decimal QuantidadeKg { get; set; }
        public DateTime DataColeta { get; set; }
        public string? Observacao { get; set; }
    }

    public class ColetaCreateViewModel
    {
        [Required(ErrorMessage = "O ponto de coleta é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um ponto de coleta válido.")]
        public int PontoColetaId { get; set; }

        [Required(ErrorMessage = "O tipo de resíduo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um tipo de resíduo válido.")]
        public int TipoResiduoId { get; set; }

        [Required(ErrorMessage = "A quantidade coletada é obrigatória.")]
        [Range(0.01, 1000000, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public decimal QuantidadeKg { get; set; }

        [StringLength(300, ErrorMessage = "A observação não pode exceder 300 caracteres.")]
        public string? Observacao { get; set; }
    }

    public class RelatorioVolumeViewModel
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal VolumeTotalKg { get; set; }
        public Dictionary<string, decimal> VolumePorTipoResiduo { get; set; } = new();
        public Dictionary<string, decimal> VolumePorPontoColeta { get; set; } = new();
    }
}
