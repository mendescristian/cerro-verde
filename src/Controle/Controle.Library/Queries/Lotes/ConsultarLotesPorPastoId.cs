using Controle.Library.Contexto;
using Controle.Library.Models.Lotes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Controle.Library.Queries.Lotes;

public record ConsultarLotesPorPastoIdQuery(int PastoId) : IRequest<List<ConsultarLoteResposta>>;

internal sealed class ConsultarLotesPorPastoIdHandler(ControleContexto contexto)
    : IRequestHandler<ConsultarLotesPorPastoIdQuery, List<ConsultarLoteResposta>>
{
    public Task<List<ConsultarLoteResposta>> Handle(
        ConsultarLotesPorPastoIdQuery query,
        CancellationToken cancellationToken
    ) =>
        contexto
            .Lotes.Where(l => l.PastoId == query.PastoId)
            .OrderBy(l => l.Numero)
            .Select(l => new ConsultarLoteResposta(
                l.Id,
                l.PastoId,
                l.Numero,
                l.Area,
                l.CapacidadeMaxima,
                l.Status,
                l.Observacoes,
                l.DataInclusao
            ))
            .ToListAsync();
}
