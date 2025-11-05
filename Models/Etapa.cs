using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Etapa
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [MaxLength(1), Column(TypeName = "char(1)")]
        public string? Status { get; set; } // e.g. N = não iniciada, E = em andamento, C = concluída

        public int? Ordem { get; set; }

        public DateTime? DtInicio { get; set; }

        public DateTime? DtFimPrevista { get; set; }

        public DateTime? DtFim { get; set; }

        [MaxLength(45)]
        public string? EtapasCol { get; set; } // campo original 'etapascol' mantido por compatibilidade

        [Required]
        public int ObraId { get; set; }

        [ForeignKey(nameof(ObraId))]
        public Obra? Obra { get; set; }

        // Navegação
        public ICollection<CustoEtapa>? CustosEtapa { get; set; }
        public ICollection<DocumentoEtapa>? DocumentosEtapa { get; set; }
    }
}
