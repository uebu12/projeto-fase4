using System.ComponentModel.DataAnnotations;

namespace Fiap.EcoColeta.Api.ViewModels
{
    public class PontoColetaViewModel
    {
        public int PontoColetaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public decimal CapacidadeMaximaKg { get; set; }
        public decimal VolumeAtualKg { get; set; }
        public decimal PercentualOcupacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }

    public class PontoColetaCreateViewModel
    {
        [Required(ErrorMessage = "O nome do ponto de coleta é obrigatório.")]
        [StringLength(120)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(200)]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Range(1, 1000000, ErrorMessage = "A capacidade máxima deve ser maior que zero.")]
        public decimal CapacidadeMaximaKg { get; set; }
    }

    public class PontoColetaUpdateViewModel
    {
        [Required(ErrorMessage = "O nome do ponto de coleta é obrigatório.")]
        [StringLength(120)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(200)]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Range(1, 1000000, ErrorMessage = "A capacidade máxima deve ser maior que zero.")]
        public decimal CapacidadeMaximaKg { get; set; }

        public bool Ativo { get; set; }
    }
}
