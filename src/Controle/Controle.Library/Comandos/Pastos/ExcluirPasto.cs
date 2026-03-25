using Controle.Library.Contexto;
using Controle.Library.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Controle.Library.Models.Pastos.ExcluirPastoResposta;
using Resposta = Controle.Library.Models.Pastos.ExcluirPastoResposta;
using TipoErro = Controle.Library.Models.Pastos.ExcluirPastoTipoErro;

namespace Controle.Library.Comandos.Pastos;

internal record ExcluirPastoComando(int PastoId) : IRequest<Resposta>;

internal sealed class ExcluirPastoHandler(
    ControleContexto contexto,
    ILogger<ExcluirPastoHandler> logger
) : IRequestHandler<ExcluirPastoComando, Resposta>
{
    public async Task<Resposta> Handle(
        ExcluirPastoComando comando,
        CancellationToken cancellationToken
    )
    {
        var pastoId = comando.PastoId;

        using var transaction = await contexto.Database.BeginTransactionAsync();

        try
        {
            var pastoExiste = await contexto.Pastos.AnyAsync(p => p.Id == pastoId);

            if (!pastoExiste)
                return new Erro(TipoErro.PastoNaoEncontrado);

            // 2. Regra de Bloqueio: Verificar se há gado ATIVO vinculado
            // Nota: Se houver gado inativo (histórico), talvez você queira bloquear também
            // ou permitir. Aqui estou bloqueando apenas se tiver animais ativos no pasto.
            var possuiGado = await contexto.Gados.AnyAsync(g =>
                g.Lote!.PastoId == pastoId && g.Status == StatusGado.Ativo
            );

            if (possuiGado)
                return new Erro(TipoErro.PastoPossuiGadoVinculado);

            await contexto.Lotes.Where(l => l.PastoId == pastoId).ExecuteDeleteAsync();

            await contexto.Pastos.Where(p => p.Id == pastoId).ExecuteDeleteAsync();

            await transaction.CommitAsync();

            return new Sucesso();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erro ao excluir pasto {PastoId}", pastoId);

            await transaction.RollbackAsync(cancellationToken);

            return new Erro(TipoErro.ErroInterno);
        }
    }
}
