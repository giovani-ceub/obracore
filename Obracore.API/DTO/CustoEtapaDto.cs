using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Obracore.Shared.DTOs
{
    public class CustoEtapaDto
    {
        public int Id { get; set; }
        [Required] public string Descricao { get; set; } = string.Empty;
        [Required] public decimal Valor { get; set; }
    }
    public class DocumentoEtapaDto
    {
        public int Id { get; set; }
        [Required] public string Titulo { get; set; } = string.Empty;
        [Required] public string Caminho { get; set; } = string.Empty;
    }
    
    // DTO Principal para criação e atualização de Etapas
    public class EtapaCreateUpdateDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O título da etapa é obrigatório.")]
        [MaxLength(100)]
        public string Titulo { get; set; } = string.Empty; 
        
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O ID da Obra é obrigatório.")]
        public int ObraId { get; set; }

        // --- NOVAS PROPRIEDADES INCLUÍDAS ---
        
        [Required(ErrorMessage = "O status da etapa é obrigatório.")]
        [MaxLength(1)]
        public string Status { get; set; } = "P"; 
        
        [Required(ErrorMessage = "A ordem da etapa é obrigatória.")]
        public int Ordem { get; set; }

        public DateTime? DtInicio { get; set; }
        
        // Data prevista para o fim da Etapa
        public DateTime? DtFimPrevista { get; set; }
        
        // Data real de conclusão
        public DateTime? DtFim { get; set; }
        
        // --- COLEÇÕES ANINHADAS ---
        
        public List<CustoEtapaDto> CustosEtapa { get; set; } = new List<CustoEtapaDto>();
        public List<DocumentoEtapaDto> DocumentosEtapa { get; set; } = new List<DocumentoEtapaDto>();
    }
}