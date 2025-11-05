using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Obracore.Models
{
    public class Documento
    {
        [Key]
        public int Id { get; set; }

        [Required, ForeignKey(nameof(Obra))]
        public int ObraId { get; set; }

        public Obra? Obra { get; set; }

        [Required, MaxLength(20)]
        public string Tipo { get; set; } = "imagem"; // imagem, projeto

        [Required, MaxLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        // Corrigido: referencia a propriedade de navegação correta
        [Required, ForeignKey(nameof(EnviadoPor))]
        public int EnviadoPorId { get; set; }

        public Usuario? EnviadoPor { get; set; }

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataEdicao { get; set; }
    }
}