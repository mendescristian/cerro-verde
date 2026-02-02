using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Movimentacao
{
    public int Id { get; init; }
    public int GadoId { get; init; }
    public int LoteOrigemId { get; init; }
    public int LoteDestinoId { get; init; }
    public TipoMovimentacao Tipo { get; init; }
    public decimal? PesoNoMomento { get; init; }
    public string ResponsavelMovimentacao { get; init; }
    public DateTimeOffset DataMovimentacao { get; init; }

    public virtual Gado? Gado { get; init; }
    public virtual Lote? LoteOrigem { get; init; }
    public virtual Lote? LoteDestino { get; init; }

    protected Movimentacao()
    {
        ResponsavelMovimentacao = null!;
    }

    public Movimentacao(
        int gadoId,
        int loteOrigemId,
        int loteDestinoId,
        DateTimeOffset dataMovimentacao,
        TipoMovimentacao tipo,
        decimal? pesoNoMomento,
        string responsavelMovimentacao
    )
    {
        GadoId = gadoId;
        LoteOrigemId = loteOrigemId;
        LoteDestinoId = loteDestinoId;
        DataMovimentacao = dataMovimentacao;
        Tipo = tipo;
        PesoNoMomento = pesoNoMomento;
        ResponsavelMovimentacao = responsavelMovimentacao;
        DataMovimentacao = DateTimeOffset.UtcNow;
    }
}
