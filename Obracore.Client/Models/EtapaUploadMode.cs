using System.Collections.Generic;

namespace Obracore.Client.Models
{
    public class EtapaUploadModel
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int ObraId { get; set; }
        public string Status { get; set; } = "P";
        public int Ordem { get; set; }

        public List<DocumentoEtapaUploadModel> Documentos { get; set; } = new();
    }
}
