using Microsoft.EntityFrameworkCore;
using Obracore.Models;

namespace Obracore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets (representam as tabelas)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; }
        public DbSet<Obra> Obras { get; set; }
        public DbSet<UsuarioObra> UsuariosObras { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<CustoObra> CustosObra { get; set; }
        public DbSet<CustoEtapa> CustosEtapas { get; set; }
        public DbSet<DocumentoObra> DocumentosObras { get; set; }
        public DbSet<DocumentoEtapa> DocumentosEtapas { get; set; }

        // ADICIONADOS (se de fato estiverem em uso)
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<Financa> Financas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =======================
            // TABELA USUARIOS
            // =======================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.Senha).HasColumnName("senha").HasMaxLength(150);
                entity.Property(e => e.Status).HasColumnName("status").HasColumnType("char(1)");
                entity.Property(e => e.DtCriacao).HasColumnName("dt_criacao");
                entity.Property(e => e.DtEdicao).HasColumnName("dt_edicao");

                // índice único em email (se desejar que e-mail seja único)
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // =======================
            // TABELA PERFIS
            // =======================
            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.ToTable("perfis");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(45);
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("text");
            });

            // =======================
            // TABELA USUARIO_PERFIL
            // =======================
            modelBuilder.Entity<UsuarioPerfil>(entity =>
            {
                entity.ToTable("usuario_perfil");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.UsuarioPerfis)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Perfil)
                      .WithMany(p => p.UsuarioPerfis)
                      .HasForeignKey(e => e.PerfilId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA OBRAS
            // =======================
            modelBuilder.Entity<Obra>(entity =>
            {
                entity.ToTable("obras");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(45);
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("text");
                entity.Property(e => e.DtInicio).HasColumnName("dt_inicio");
                entity.Property(e => e.DtFimPrevista).HasColumnName("dt_fim_prevista");
                entity.Property(e => e.DtFim).HasColumnName("dt_fim");
                entity.Property(e => e.DtCriacao).HasColumnName("dt_criacao");
                entity.Property(e => e.DtEdicao).HasColumnName("dt_edicao");
                entity.Property(e => e.StatusObra).HasColumnName("status_obra").HasColumnType("char(1)");
            });

            // =======================
            // TABELA USUARIOS_OBRAS
            // =======================
            modelBuilder.Entity<UsuarioObra>(entity =>
            {
                entity.ToTable("usuarios_obras");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.UsuariosObras)
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Obra)
                      .WithMany(o => o.UsuariosObras)
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA ETAPAS
            // =======================
            modelBuilder.Entity<Etapa>(entity =>
            {
                entity.ToTable("etapas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasColumnName("nome").HasMaxLength(100);
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("text");
                entity.Property(e => e.Status).HasColumnName("status").HasColumnType("char(1)");
                entity.Property(e => e.Ordem).HasColumnName("ordem");
                entity.Property(e => e.DtInicio).HasColumnName("dt_inicio");
                entity.Property(e => e.DtFimPrevista).HasColumnName("dt_fim_prevista");
                entity.Property(e => e.DtFim).HasColumnName("dt_fim");
                entity.Property(e => e.EtapasCol).HasColumnName("etapascol").HasMaxLength(45);
                entity.Property(e => e.ObraId).HasColumnName("obras_id");

                entity.HasOne(e => e.Obra)
                      .WithMany(o => o.Etapas)
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA CUSTOS_OBRA
            // =======================
            modelBuilder.Entity<CustoObra>(entity =>
            {
                entity.ToTable("custos_obra");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("text");
                // usar HasPrecision para garantir precisão
                entity.Property(e => e.Valor).HasColumnName("valor").HasPrecision(12, 2);
                entity.Property(e => e.DtRegistro).HasColumnName("dt_registro");
                entity.Property(e => e.ObraId).HasColumnName("obras_id");
                entity.Property(e => e.EtapaId).HasColumnName("etapas_id");

                entity.HasOne(e => e.Obra)
                      .WithMany(o => o.CustosObra)
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);

                // relação opcional com etapa: deixar claro comportamento ao deletar
                entity.HasOne(e => e.Etapa)
                      .WithMany()
                      .HasForeignKey(e => e.EtapaId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // =======================
            // TABELA CUSTOS_ETAPAS
            // =======================
            modelBuilder.Entity<CustoEtapa>(entity =>
            {
                entity.ToTable("custos_etapas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("text");
                entity.Property(e => e.Valor).HasColumnName("valor").HasPrecision(12, 2);
                entity.Property(e => e.DtRegistro).HasColumnName("dt_registro");
                entity.Property(e => e.EtapaId).HasColumnName("etapas_id");

                entity.HasOne(e => e.Etapa)
                      .WithMany(e => e.CustosEtapa)
                      .HasForeignKey(e => e.EtapaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA DOCUMENTOS_OBRAS
            // =======================
            modelBuilder.Entity<DocumentoObra>(entity =>
            {
                entity.ToTable("documentos_obras");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(100);
                entity.Property(e => e.Caminho).HasColumnName("caminho").HasMaxLength(255);
                entity.Property(e => e.DtCriacao).HasColumnName("dt_criacao");
                entity.Property(e => e.ObraId).HasColumnName("obras_id");

                entity.HasOne(e => e.Obra)
                      .WithMany(o => o.DocumentosObras)
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA DOCUMENTOS_ETAPA
            // =======================
            modelBuilder.Entity<DocumentoEtapa>(entity =>
            {
                entity.ToTable("documentos_etapa");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(100);
                entity.Property(e => e.Caminho).HasColumnName("caminho").HasMaxLength(255);
                entity.Property(e => e.DtCriacao).HasColumnName("dt_criacao");
                entity.Property(e => e.EtapaId).HasColumnName("etapas_id");

                entity.HasOne(e => e.Etapa)
                      .WithMany(e => e.DocumentosEtapa)
                      .HasForeignKey(e => e.EtapaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // TABELA DOCUMENTOS (GENÉRICO) - ADICIONADO
            // =======================
            modelBuilder.Entity<Documento>(entity =>
            {
                entity.ToTable("documentos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(20);
                entity.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(150);
                entity.Property(e => e.CaminhoArquivo).HasColumnName("caminho_arquivo").HasMaxLength(255);
                entity.Property(e => e.DataCriacao).HasColumnName("data_criacao");
                entity.Property(e => e.DataEdicao).HasColumnName("data_edicao");
                entity.Property(e => e.ObraId).HasColumnName("obras_id");
                entity.Property(e => e.EnviadoPorId).HasColumnName("enviado_por_id");

                // se quiser que Documento seja ligado à coleção DocumentosObras, ajuste Obra model; aqui deixo sem WithMany para não quebrar
                entity.HasOne(e => e.Obra)
                      .WithMany()
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.EnviadoPor)
                      .WithMany()
                      .HasForeignKey(e => e.EnviadoPorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =======================
            // TABELA FINANCA - ADICIONADA
            // =======================
            modelBuilder.Entity<Financa>(entity =>
            {
                entity.ToTable("financas");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ObraId).HasColumnName("obras_id");
                entity.Property(e => e.EtapaId).HasColumnName("etapas_id");
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(255);
                entity.Property(e => e.Valor).HasColumnName("valor").HasPrecision(12, 2);
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(20);
                entity.Property(e => e.DataRegistro).HasColumnName("data_registro");
                entity.Property(e => e.DataCriacao).HasColumnName("data_criacao");
                entity.Property(e => e.DataEdicao).HasColumnName("data_edicao");

                entity.HasOne(e => e.Obra)
                      .WithMany()
                      .HasForeignKey(e => e.ObraId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Etapa)
                      .WithMany()
                      .HasForeignKey(e => e.EtapaId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}