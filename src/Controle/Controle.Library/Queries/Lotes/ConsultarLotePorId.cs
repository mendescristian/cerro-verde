using Controle.Library.Contexto;
using Controle.Library.Models.Lotes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Controle.Library.Queries.Lotes;

public record ConsultarLotePorIdQuery(int LoteId) : IRequest<ConsultarLoteResposta?>;

internal sealed class ConsultarLotePorIdHandler(ControleContexto contexto)
    : IRequestHandler<ConsultarLotePorIdQuery, ConsultarLoteResposta?>
{
    public Task<ConsultarLoteResposta?> Handle(
        ConsultarLotePorIdQuery query,
        CancellationToken cancellationToken
    ) =>
        contexto
            .Lotes.Where(l => l.Id == query.LoteId)
            .Select(l => new ConsultarLoteResposta(
                l.Id,
                l.PastoId,
                l.Numero,
                l.Area,
                l.CapacidadeMaxima,
                l.Status,
                l.Observacoes,
                l.DataInclusao,
                l.DataUltimaAtualizacao
            ))
            .FirstOrDefaultAsync();
}
