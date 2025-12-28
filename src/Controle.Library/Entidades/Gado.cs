using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Gado
{
    public int Id { get; init; }
    public int PastoId { get; init; }
    public int Brinco { get; init; }
    public bool Valido { get; init; }
    public StatusGado Status { get; init; }
    public string? Nome { get; init; }
    public SexoGado Sexo { get; init; }
    public RacaGado Raca { get; init; }
    public DateTimeOffset DataNascimento { get; init; }
    public decimal? PesoNascimento { get; init; }
    public decimal PesoAtual { get; init; }
    public DateTimeOffset DataUltimaPesagem { get; init; }
    public int? PaiId { get; init; }
    public int? MaeId { get; init; }
    public OrigemGado Origem { get; init; }
    public GadoDadosEntrada? DadosEntrada { get; init; }
    public FinalidadeGado Finalidade { get; init; }
    public GadoDadosSaida? DadosSaida { get; init; }
    public GadoDadosPrenhez? DadosPrenhez { get; init; }
    public string? Observacoes { get; init; }
    public DateTimeOffset DataInclusao { get; private set; }
    public DateTimeOffset? DataUltimaAtualizacao { get; init; }

    public virtual Pasto? Pasto { get; init; }
    public virtual ICollection<Tratamento>? Tratamentos { get; init; }

    protected Gado() { }

    public Gado(
        int pastoId,
        int brinco,
        SexoGado sexo,
        RacaGado raca,
        DateTimeOffset dataNascimento,
        decimal pesoAtual,
        int? paiId,
        int? maeId,
        OrigemGado origem,
        GadoDadosEntrada? dadosEntrada,
        FinalidadeGado finalidade,
        string? observacoes
    )
    {
        PastoId = pastoId;
        Brinco = brinco;
        Valido = true;
        Status = StatusGado.Ativo;
        Sexo = sexo;
        Raca = raca;
        DataNascimento = dataNascimento;
        PesoAtual = pesoAtual;
        DataUltimaPesagem = DateTimeOffset.UtcNow;
        PaiId = paiId;
        MaeId = maeId;
        Origem = origem;
        DadosEntrada = dadosEntrada;
        Finalidade = finalidade;
        DataInclusao = DateTimeOffset.UtcNow;
        Observacoes = observacoes;
    }
}

public record GadoDadosEntrada(string Origem, long? ValorCompra, DateTimeOffset Data);

public record GadoDadosSaida(string Motivo, DateTimeOffset Data);

public record GadoDadosPrenhez(OrigemPrenhez Origem, DateTimeOffset DataPrevisaoParto);
