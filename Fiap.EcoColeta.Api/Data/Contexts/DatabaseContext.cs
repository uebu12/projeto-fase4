using Fiap.EcoColeta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fiap.EcoColeta.Api.Data.Contexts
{
    /// <summary>
    /// Contexto do Entity Framework Core. Mapeamento escrito de forma
    /// agnóstica de provedor (usa HasPrecision em vez de HasColumnType
    /// fixo), permitindo alternar entre Oracle (oracle.fiap.com.br) e
    /// SQL Server (LocalDB) apenas trocando o provedor no Program.cs e
    /// a connection string no appsettings — sem precisar reescrever o
    /// mapeamento das entidades.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<TipoResiduoModel> TiposResiduo { get; set; }
        public virtual DbSet<PontoColetaModel> PontosColeta { get; set; }
        public virtual DbSet<ColetaModel> Coletas { get; set; }
        public virtual DbSet<AlertaModel> Alertas { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected DatabaseContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===================== TipoResiduo =====================
            modelBuilder.Entity<TipoResiduoModel>(entity =>
            {
                entity.ToTable("ECO_TIPO_RESIDUO");
                entity.HasKey(e => e.TipoResiduoId);

                entity.Property(e => e.TipoResiduoId).HasColumnName("TP_RESIDUO_ID");
                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .IsRequired()
                    .HasMaxLength(80);
                entity.Property(e => e.FatorImpactoCO2)
                    .HasColumnName("FATOR_IMPACTO_CO2")
                    .HasPrecision(10, 4);
                entity.Property(e => e.Reciclavel).HasColumnName("RECICLAVEL");

                entity.HasIndex(e => e.Nome).IsUnique();

                // Seed inicial: tipos de resíduo mais comuns com fator de
                // impacto de CO2 evitado (kg CO2 por kg reciclado), usado
                // pelos relatórios ESG desde o primeiro deploy.
                entity.HasData(
                    new TipoResiduoModel { TipoResiduoId = 1, Nome = "Papel/Papelão", FatorImpactoCO2 = 0.9m, Reciclavel = true },
                    new TipoResiduoModel { TipoResiduoId = 2, Nome = "Plástico", FatorImpactoCO2 = 1.5m, Reciclavel = true },
                    new TipoResiduoModel { TipoResiduoId = 3, Nome = "Vidro", FatorImpactoCO2 = 0.3m, Reciclavel = true },
                    new TipoResiduoModel { TipoResiduoId = 4, Nome = "Metal/Alumínio", FatorImpactoCO2 = 9.0m, Reciclavel = true },
                    new TipoResiduoModel { TipoResiduoId = 5, Nome = "Eletrônico", FatorImpactoCO2 = 1.2m, Reciclavel = true },
                    new TipoResiduoModel { TipoResiduoId = 6, Nome = "Orgânico", FatorImpactoCO2 = 0.5m, Reciclavel = true }
                );
            });

            // ===================== PontoColeta =====================
            modelBuilder.Entity<PontoColetaModel>(entity =>
            {
                entity.ToTable("ECO_PONTO_COLETA");
                entity.HasKey(e => e.PontoColetaId);

                entity.Property(e => e.PontoColetaId).HasColumnName("PT_COLETA_ID");
                entity.Property(e => e.Nome).HasColumnName("NOME").IsRequired().HasMaxLength(120);
                entity.Property(e => e.Endereco).HasColumnName("ENDERECO").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Bairro).HasColumnName("BAIRRO").IsRequired().HasMaxLength(100);
                entity.Property(e => e.CapacidadeMaximaKg)
                    .HasColumnName("CAPACIDADE_MAXIMA_KG")
                    .HasPrecision(10, 2);
                entity.Property(e => e.VolumeAtualKg)
                    .HasColumnName("VOLUME_ATUAL_KG")
                    .HasPrecision(10, 2);
                entity.Property(e => e.Ativo).HasColumnName("ATIVO");
                entity.Property(e => e.DataCadastro).HasColumnName("DATA_CADASTRO");
            });

            // ===================== Coleta =====================
            modelBuilder.Entity<ColetaModel>(entity =>
            {
                entity.ToTable("ECO_COLETA");
                entity.HasKey(e => e.ColetaId);

                entity.Property(e => e.ColetaId).HasColumnName("COLETA_ID");
                entity.Property(e => e.QuantidadeKg)
                    .HasColumnName("QUANTIDADE_KG")
                    .HasPrecision(10, 2)
                    .IsRequired();
                entity.Property(e => e.DataColeta).HasColumnName("DATA_COLETA");
                entity.Property(e => e.Observacao).HasColumnName("OBSERVACAO").HasMaxLength(300);

                entity.HasOne(e => e.PontoColeta)
                    .WithMany(p => p.Coletas)
                    .HasForeignKey(e => e.PontoColetaId)
                    .IsRequired();

                entity.HasOne(e => e.TipoResiduo)
                    .WithMany(t => t.Coletas)
                    .HasForeignKey(e => e.TipoResiduoId)
                    .IsRequired();

                entity.HasIndex(e => e.DataColeta);
            });

            // ===================== Alerta =====================
            modelBuilder.Entity<AlertaModel>(entity =>
            {
                entity.ToTable("ECO_ALERTA");
                entity.HasKey(e => e.AlertaId);

                entity.Property(e => e.AlertaId).HasColumnName("ALERTA_ID");
                entity.Property(e => e.Mensagem).HasColumnName("MENSAGEM").IsRequired().HasMaxLength(300);
                entity.Property(e => e.PercentualOcupacao)
                    .HasColumnName("PERCENTUAL_OCUPACAO")
                    .HasPrecision(5, 2);
                entity.Property(e => e.DataAlerta).HasColumnName("DATA_ALERTA");
                entity.Property(e => e.Resolvido).HasColumnName("RESOLVIDO");
                entity.Property(e => e.DataResolucao).HasColumnName("DATA_RESOLUCAO");

                entity.HasOne(e => e.PontoColeta)
                    .WithMany(p => p.Alertas)
                    .HasForeignKey(e => e.PontoColetaId)
                    .IsRequired();
            });
        }
    }
}
