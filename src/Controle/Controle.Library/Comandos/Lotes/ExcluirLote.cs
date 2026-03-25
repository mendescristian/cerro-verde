using Controle.Library.Contexto;
using Controle.Library.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Controle.Library.Models.Lotes.ExcluirLoteResposta;
using Resposta = Controle.Library.Models.Lotes.ExcluirLoteResposta;
using TipoErro = Controle.Library.Models.Lotes.ExcluirLoteTipoErro;

namespace Controle.Library.Comandos.Lotes;

internal record ExcluirLoteComando(int LoteId) : IRequest<Resposta>;

internal sealed class ExcluirLoteHandler(
    ControleContexto contexto,
    ILogger<ExcluirLoteHandler> logger
) : IRequestHandler<ExcluirLoteComando, Resposta>
{
    public async Task<Resposta> Handle(
        ExcluirLoteComando comando,
        CancellationToken cancellationToken
    )
    {
        var loteId = comando.LoteId;

        try
        {
            var loteExiste = await contexto.Lotes.AnyAsync(l => l.Id == loteId);

            if (!loteExiste)
                return TipoErro.LoteNaoEncontrado;

            var possuiGado = await contexto.Gados.AnyAsync(g =>
                g.LoteId == loteId && g.Status == StatusGado.Ativo
            );

            if (possuiGado)
                return new Erro(TipoErro.LotePossuiGadoVinculado);

            await contexto.Lotes.Where(l => l.Id == loteId).ExecuteDeleteAsync();

            return new Sucesso();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erro ao excluir lote {LoteId}", loteId);

            return TipoErro.ErroInterno;
        }
    }
}
