using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Obra
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(45)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public DateTime? DtInicio { get; set; }

        public DateTime? DtFimPrevista { get; set; }

        public DateTime? DtFim { get; set; } // data final real

        [Required]
        public DateTime DtCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DtEdicao { get; set; }

        [MaxLength(1), Column(TypeName = "char(1)")]
        public string? StatusObra { get; set; } // ex: 'A' ativa, 'F' finalizada

        [MaxLength(255)]
        public string? FotoCapa { get; set; } // Caminho relativo ou URL da imagem


        // Navegação
        public ICollection<UsuarioObra>? UsuariosObras { get; set; }
        public ICollection<DocumentoObra>? DocumentosObras { get; set; }
        public ICollection<CustoObra>? CustosObra { get; set; }
        public ICollection<Etapa>? Etapas { get; set; }
    }
}