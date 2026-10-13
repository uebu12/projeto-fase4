using System.ComponentModel.DataAnnotations;

namespace Fiap.EcoColeta.Api.ViewModels
{
    public class TipoResiduoViewModel
    {
        public int TipoResiduoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal FatorImpactoCO2 { get; set; }
        public bool Reciclavel { get; set; }
    }

    public class TipoResiduoCreateViewModel
    {
        [Required(ErrorMessage = "O nome do tipo de resíduo é obrigatório.")]
        [StringLength(80, ErrorMessage = "O nome não pode exceder 80 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 1000, ErrorMessage = "O fator de impacto de CO2 deve estar entre 0 e 1000.")]
        public decimal FatorImpactoCO2 { get; set; }

        public bool Reciclavel { get; set; } = true;
    }
}
