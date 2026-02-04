using Controle.Library.Contexto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Requisicao = Controle.Library.Models.Lotes.AtualizarLoteRequisicao;
using Resposta = Controle.Library.Models.Lotes.AtualizarLoteResposta;
using TipoErro = Controle.Library.Models.Lotes.AtualizarLoteTipoErro;

namespace Controle.Library.Comandos.Lotes;

internal record AtualizarLoteComando(Requisicao Requisicao) : IRequest<Resposta>;

internal sealed class AtualizarLoteHandler(
    ControleContexto contexto,
    ILogger<AtualizarLoteHandler> logger,
    TimeProvider timeProvider
) : IRequestHandler<AtualizarLoteComando, Resposta>
{
    public async Task<Resposta> Handle(
        AtualizarLoteComando comando,
        CancellationToken cancellationToken
    )
    {
        var requisicao = comando.Requisicao;

        try
        {
            var existeLote = await contexto.Lotes.AnyAsync(l => l.Id == requisicao.LoteId);

            if (!existeLote)
                return TipoErro.LoteNaoEncontrado;

            await contexto
                .Lotes.Where(l => l.Id == requisicao.LoteId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(l => l.Area, requisicao.Area)
                        .SetProperty(l => l.CapacidadeMaxima, requisicao.CapacidadeMaxima)
                        .SetProperty(l => l.Status, requisicao.Status)
                        .SetProperty(l => l.Observacoes, requisicao.Observacoes)
                        .SetProperty(l => l.DataUltimaAtualizacao, timeProvider.GetUtcNow())
                );

            return requisicao.LoteId;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erro ao atualizar lote {LoteId}", requisicao.LoteId);

            return TipoErro.ErroInterno;
        }
    }
}
