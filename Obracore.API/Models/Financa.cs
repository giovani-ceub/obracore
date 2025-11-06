using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Financa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Obra")]
        public int ObraId { get; set; }

        public Obra? Obra { get; set; }

        [ForeignKey("Etapa")]
        public int? EtapaId { get; set; }

        public Etapa? Etapa { get; set; }

        [Required, MaxLength(255)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public decimal Valor { get; set; }

        [Required, MaxLength(20)]
        public string Tipo { get; set; } = "despesa"; // despesa, investimento

        [Required]
        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataEdicao { get; set; }
    }
}
