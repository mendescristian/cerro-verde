using System;
using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Movimentacao
{
    public int Id { get; init; }
    public int GadoId { get; init; }
    public int PastoOrigemId { get; init; }
    public int PastoDestinoId { get; init; }
    public TipoMovimentacao Tipo { get; init; }
    public decimal? PesoNoMomento { get; init; }
    public string ResponsavelMovimentacao { get; init; }
    public DateTimeOffset DataMovimentacao { get; init; }

    public virtual Gado? Gado { get; init; }
    public virtual Pasto? PastoOrigem { get; init; }
    public virtual Pasto? PastoDestino { get; init; }

    protected Movimentacao()
    {
        ResponsavelMovimentacao = null!;
    }

    public Movimentacao(
        int gadoId,
        int pastoOrigemId,
        int pastoDestinoId,
        DateTimeOffset dataMovimentacao,
        TipoMovimentacao tipo,
        decimal? pesoNoMomento,
        string responsavelMovimentacao
    )
    {
        GadoId = gadoId;
        PastoOrigemId = pastoOrigemId;
        PastoDestinoId = pastoDestinoId;
        DataMovimentacao = dataMovimentacao;
        Tipo = tipo;
        PesoNoMomento = pesoNoMomento;
        ResponsavelMovimentacao = responsavelMovimentacao;
        DataMovimentacao = DateTimeOffset.UtcNow;
    }
}
