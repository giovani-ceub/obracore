using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Obracore.API.DTO
{
    public class EtapaUploadDto
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int ObraId { get; set; }
        public string Status { get; set; } = "P";
        public int Ordem { get; set; }

        // Upload real de arquivos
        public List<IFormFile>? Documentos { get; set; }
    }
}
