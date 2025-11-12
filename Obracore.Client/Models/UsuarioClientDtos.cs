using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Obracore.Client.Models
{
    // DTO usado para popular o dropdown de perfis
    public class PerfilDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }

    // DTO para a listagem geral (GET api/Usuarios)
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Status { get; set; }
        public DateTime DtCriacao { get; set; }
        public DateTime? DtEdicao { get; set; }
        // Campo formatado pelo backend para exibição na tabela
        public string? PerfisDisplay { get; set; } 
    }

    // Corresponde a UsuarioCreateDto no Backend (entrada POST)
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;
        
        public string Status { get; set; } = "A"; // Default 'A'tivo

        public List<int> PerfilIds { get; set; } = new List<int>(); 
    }

    // Corresponde a UsuarioEditDto no Backend (saída GET por ID)
    public class UsuarioEditDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;
        
        public string? Status { get; set; }

        public List<int> PerfilIds { get; set; } = new List<int>(); 
    }

    // Corresponde a UsuarioUpdateDto no Backend (entrada PUT)
    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = string.Empty;
        
        public string? Status { get; set; }
        
        public string? NovaSenha { get; set; } 

        public List<int> PerfilIds { get; set; } = new List<int>(); 
    }
}