using Controle.Library.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Controle.Library.Contexto;

internal class ControleContexto : DbContext
{
    public DbSet<Pasto> Pastos => Set<Pasto>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<Gado> Gados => Set<Gado>();
    public DbSet<Tratamento> Tratamentos => Set<Tratamento>();
    public DbSet<Movimentacao> Movimentacoes => Set<Movimentacao>();

    public ControleContexto() { }

    public ControleContexto(DbContextOptions<ControleContexto> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfiguracaoPasto).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetConverter>();
    }
}

public static class ConfiguradorControleContexto
{
    public static DbContextOptionsBuilder ConfigurarContexto(
        this DbContextOptionsBuilder builder,
        string connectionString
    ) =>
        builder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}

internal class ConfiguracaoPasto : IEntityTypeConfiguration<Pasto>
{
    public void Configure(EntityTypeBuilder<Pasto> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Nome).HasMaxLength(50);

        builder.Property(g => g.Area);

        builder.Property(g => g.TipoPasto);

        builder.Property(g => g.CapacidadeMaxima);

        builder.Property(g => g.TemAguadouro);

        builder.Property(g => g.TemCochoSal);

        builder.Property(g => g.TemSombra);

        builder.Property(g => g.UltimaReforma);

        builder.Property(g => g.ProximaReforma);

        builder.Property(g => g.Status);

        builder.Property(g => g.Observacoes).HasMaxLength(200);

        builder.Property(g => g.DataInclusao);

        builder.Property(g => g.DataUltimaAtualizacao);

        builder.OwnsOne(
            g => g.DadosAguadouro,
            aguadouroBuilder =>
            {
                aguadouroBuilder.Property(e => e.DataUltimaAfericao);

                aguadouroBuilder.Property(e => e.QuantidadeAferida);

                aguadouroBuilder.Property(e => e.ResponsavelAfericao).HasMaxLength(50);
            }
        );

        builder.OwnsOne(
            g => g.DadosCochoSal,
            cochoSalBuilder =>
            {
                cochoSalBuilder.Property(e => e.DataUltimaAfericao);

                cochoSalBuilder.Property(e => e.QuantidadeAferida);

                cochoSalBuilder.Property(e => e.ResponsavelAfericao).HasMaxLength(50);
            }
        );

        builder.HasMany(p => p.Lotes).WithOne(l => l.Pasto).HasForeignKey(l => l.PastoId);

        builder.HasIndex(d => d.Nome).IsUnique();
    }
}

internal class ConfiguracaoLote : IEntityTypeConfiguration<Lote>
{
    public void Configure(EntityTypeBuilder<Lote> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Numero);

        builder.Property(l => l.Area).HasPrecision(18, 2);

        builder.Property(l => l.Status);

        builder.Property(l => l.Observacoes).HasMaxLength(200);

        builder
            .HasOne(l => l.Pasto)
            .WithMany(p => p.Lotes)
            .HasForeignKey(l => l.PastoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Gados).WithOne(g => g.Lote).HasForeignKey(g => g.LoteId);

        builder.HasIndex(l => new { l.PastoId, l.Numero }).IsUnique();
    }
}

internal class ConfiguracaoGado : IEntityTypeConfiguration<Gado>
{
    public void Configure(EntityTypeBuilder<Gado> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.LoteId);

        builder.Property(g => g.Brinco);

        builder.Property(g => g.Valido);

        builder.Property(g => g.Status);

        builder.Property(g => g.Sexo);

        builder.Property(g => g.Raca);

        builder.Property(g => g.DataNascimento);

        builder.Property(g => g.PesoAtual).HasPrecision(18, 4);

        builder.Property(g => g.PesoAtual).HasPrecision(18, 4);

        builder.Property(g => g.DataUltimaPesagem);

        builder.Property(g => g.PaiId);

        builder.Property(g => g.MaeId);

        builder.Property(g => g.Origem);

        builder.Property(g => g.Finalidade);

        builder.Property(g => g.Prenhez);

        builder.Property(g => g.Observacoes).HasMaxLength(200);

        builder.Property(g => g.DataInclusao);

        builder.Property(g => g.DataUltimaAtualizacao);

        builder.OwnsOne(
            g => g.DadosEntrada,
            dadosEntradaBuilder =>
            {
                dadosEntradaBuilder.Property(e => e.Origem);

                dadosEntradaBuilder.Property(e => e.ValorCompra).HasPrecision(18, 4);

                dadosEntradaBuilder.Property(e => e.Data);
            }
        );

        builder.OwnsOne(
            g => g.DadosSaida,
            dadosSaidaBuilder =>
            {
                dadosSaidaBuilder.Property(e => e.Motivo).HasMaxLength(200);

                dadosSaidaBuilder.Property(e => e.Data);
            }
        );

        builder.OwnsOne(
            g => g.DadosPrenhez,
            dadosPrenhezBuilder =>
            {
                dadosPrenhezBuilder.Property(e => e.Origem);

                dadosPrenhezBuilder.Property(e => e.DataPrevisaoParto);
            }
        );

        builder.HasMany(i => i.Tratamentos).WithOne(g => g.Gado).HasForeignKey(g => g.GadoId);

        builder.HasIndex(g => new { g.Brinco }).IsUnique().HasFilter("valido = true");

        builder.HasIndex(g => new { g.LoteId, g.Sexo }).HasDatabaseName("ix_gados_lote_id_sexo");
    }
}

internal class ConfiguracaoTratamento : IEntityTypeConfiguration<Tratamento>
{
    public void Configure(EntityTypeBuilder<Tratamento> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.GadoId);

        builder.Property(g => g.Tipo);

        builder.Property(g => g.NomeProduto).HasMaxLength(100);

        builder.Property(g => g.Fabricante).HasMaxLength(100);

        builder.Property(g => g.Lote).HasMaxLength(50);

        builder.Property(g => g.DataAplicacao);

        builder.Property(g => g.DataProximaAplicacao);

        builder.Property(g => g.Dose).HasMaxLength(15);

        builder.Property(g => g.TipoAplicacao);

        builder.Property(g => g.ResponsavelAplicacao).HasMaxLength(50);

        builder.Property(g => g.Motivo).HasMaxLength(200);

        builder.Property(g => g.Observacoes).HasMaxLength(200);

        builder.Property(g => g.DataInclusao);

        builder.Property(g => g.DataUltimaAtualizacao);

        builder
            .HasIndex(g => new { g.GadoId, g.DataAplicacao })
            .HasDatabaseName("ix_tratamentos_gado_id_data_aplicacao");
    }
}

internal class ConfiguracaoMovimentacao : IEntityTypeConfiguration<Movimentacao>
{
    public void Configure(EntityTypeBuilder<Movimentacao> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.GadoId);

        builder.Property(m => m.LoteOrigemId);

        builder.Property(m => m.LoteDestinoId);

        builder.Property(m => m.Tipo);

        builder.Property(m => m.PesoNoMomento).HasPrecision(18, 4);

        builder.Property(m => m.ResponsavelMovimentacao).HasMaxLength(50);

        builder.Property(m => m.DataMovimentacao);

        builder.HasOne(m => m.Gado).WithMany(g => g.Movimentacoes).HasForeignKey(m => m.GadoId);

        builder.HasOne(m => m.LoteOrigem).WithMany().HasForeignKey(m => m.LoteOrigemId);

        builder.HasOne(m => m.LoteDestino).WithMany().HasForeignKey(m => m.LoteOrigemId);

        builder
            .HasIndex(m => new { m.GadoId, m.DataMovimentacao })
            .HasDatabaseName("ix_movimentacoes_gado_id_data_movimentacao");

        builder
            .HasIndex(m => new { m.LoteOrigemId, m.DataMovimentacao })
            .HasDatabaseName("ix_movimentacoes_lote_origem_id_data");

        builder
            .HasIndex(m => new { m.LoteDestinoId, m.DataMovimentacao })
            .HasDatabaseName("ix_movimentacoes_lote_destino_id_data");
    }
}

public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(d => d.ToUniversalTime(), d => d.ToUniversalTime()) { }
}
