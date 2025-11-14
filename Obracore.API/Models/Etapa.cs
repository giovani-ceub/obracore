using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Obracore.Models
{
    public class Etapa
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty; 

        public string? Descricao { get; set; }

        public string Status { get; set; } = "A"; // Ex: A (Ativa), C (Concluída), P (Pendente)

        // Nova propriedade: Mapeia para a coluna 'ordem'
        public int Ordem { get; set; }

        // Data de Início Real
        public DateTime? DtInicio { get; set; }

        // Nova propriedade: Mapeia para a coluna 'dt_fim_prevista'
        public DateTime? DtFimPrevista { get; set; }
        
        // Renomeado de DtFimReal para DtFim, para mapear com 'dt_fim' no DB.
        public DateTime? DtFim { get; set; } 

        // Coluna extra que estava no seu DbContext, mantida por segurança.
        public string? EtapasCol { get; set; }

        // CHAVE ESTRANGEIRA (FK)
        public int ObraId { get; set; }

        // PROPRIEDADES DE NAVEGAÇÃO
        public Obra? Obra { get; set; } 

        // Coleções (Relacionamentos One-to-Many)
        public ICollection<CustoEtapa> CustosEtapa { get; set; } = new List<CustoEtapa>();
        public ICollection<DocumentoEtapa> DocumentosEtapa { get; set; } = new List<DocumentoEtapa>();
        
    }
}