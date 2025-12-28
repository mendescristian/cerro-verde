using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Pasto
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public decimal Area { get; init; }
    public TipoPasto TipoPasto { get; init; }
    public int CapacidadeMaxima { get; init; }
    public bool TemAguadouro { get; init; }
    public DadosAguadouroPasto? DadosAguadouro { get; init; }
    public bool TemCochoSal { get; init; }
    public DadosCochoSalPasto? DadosCochoSal { get; init; }
    public bool TemSombra { get; init; }
    public DateTimeOffset? UltimaReforma { get; init; }
    public DateTimeOffset? ProximaReforma { get; init; }
    public StatusPasto Status { get; init; }
    public string? Observacoes { get; init; }
    public DateTimeOffset DataInclusao { get; private set; }
    public DateTimeOffset? DataUltimaAtualizacao { get; init; }

    public virtual ICollection<Gado>? Gados { get; init; }

    protected Pasto()
    {
        Nome = null!;
    }

    public Pasto(
        string nome,
        decimal area,
        TipoPasto tipoPasto,
        int capacidadeMaxima,
        bool temAguadouro,
        bool temCochoSal,
        bool temSombra,
        string? observacoes
    )
    {
        Nome = nome;
        Area = area;
        TipoPasto = tipoPasto;
        CapacidadeMaxima = capacidadeMaxima;
        TemAguadouro = temAguadouro;
        TemCochoSal = temCochoSal;
        TemSombra = temSombra;
        Status = StatusPasto.Ativo;
        Observacoes = observacoes;
        DataInclusao = DateTimeOffset.UtcNow;
    }
}

public record DadosAguadouroPasto(
    DateTimeOffset DataUltimaAfericao,
    decimal QuantidadeAferida,
    string ResponsavelAfericao
);

public record DadosCochoSalPasto(
    DateTimeOffset DataUltimaAfericao,
    decimal QuantidadeAferida,
    string ResponsavelAfericao
);
