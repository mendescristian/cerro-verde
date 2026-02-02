using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Lote
{
    public int Id { get; init; }
    public int PastoId { get; init; }
    public int Numero { get; init; }
    public decimal Area { get; init; }
    public int CapacidadeMaxima { get; init; }
    public StatusLote Status { get; init; }
    public string? Observacoes { get; init; }
    public DateTimeOffset DataInclusao { get; private set; }
    public DateTimeOffset? DataUltimaAtualizacao { get; init; }

    public virtual Pasto? Pasto { get; init; }
    public virtual ICollection<Gado>? Gados { get; init; }

    public Lote(int pastoId, int numero, decimal area, int capacidadeMaxima, string? observacoes)
    {
        PastoId = pastoId;
        Numero = numero;
        Area = area;
        CapacidadeMaxima = capacidadeMaxima;
        Status = StatusLote.EmDescanso;
        Observacoes = observacoes;
        DataInclusao = DateTimeOffset.UtcNow;
    }
}
