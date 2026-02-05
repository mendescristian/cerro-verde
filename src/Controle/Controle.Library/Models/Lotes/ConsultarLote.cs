using Controle.Library.Enums;

namespace Controle.Library.Models.Lotes;

public record ConsultarLoteResposta(
    int Id,
    int PastoId,
    int Numero,
    decimal Area,
    int CapacidadeMaxima,
    StatusLote Status,
    string? Observacoes,
    DateTimeOffset DataInclusao
);
