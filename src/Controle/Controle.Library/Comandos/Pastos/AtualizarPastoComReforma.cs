using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Controle.Library.Models.Pastos.AtualizarPastoComReformaRequisicao;
using static Controle.Library.Models.Pastos.AtualizarPastoComReformaResposta;
using Requisicao = Controle.Library.Models.Pastos.AtualizarPastoComReformaRequisicao;
using Resposta = Controle.Library.Models.Pastos.AtualizarPastoComReformaResposta;
using TipoErro = Controle.Library.Models.Pastos.AtualizarPastoComReformaTipoErro;

namespace Controle.Library.Comandos.Pastos;

internal record AtualizarPastoComReformaComando(Requisicao Requisicao) : IRequest<Resposta>;

internal sealed class AtualizarPastoComReformaHandler(
    ControleContexto contexto,
    ILogger<AtualizarPastoComReformaHandler> logger,
    TimeProvider timeProvider
) : IRequestHandler<AtualizarPastoComReformaComando, Resposta>
{
    public async Task<Resposta> Handle(
        AtualizarPastoComReformaComando comando,
        CancellationToken cancellationToken
    )
    {
        var requisicao = comando.Requisicao;

        using var transaction = await contexto.Database.BeginTransactionAsync();

        try
        {
            var existePasto = await contexto.Pastos.AnyAsync(p => p.Id == requisicao.PastoId);
            if (!existePasto)
                return TipoErro.PastoNaoEncontrado;

            var resposta = requisicao switch
            {
                Programada programada => await AtualizarProgramadaAsync(programada),
                Vigente vigente => await AtualizarVigenteAsync(vigente),
                Finalizada finalizada => await AtualizarFinalizadaAsync(finalizada),
            };

            var finalizacaoTransacao = resposta switch
            {
                Sucesso => transaction.CommitAsync(),
                Erro => transaction.RollbackAsync(),
            };

            await finalizacaoTransacao;

            return resposta;
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erro ao atualizar pasto com reforma {Requisicao}", requisicao);

            await transaction.RollbackAsync();

            return TipoErro.ErroInterno;
        }
    }

    private async Task<Resposta> AtualizarProgramadaAsync(Programada requisicao)
    {
        await contexto
            .Pastos.Where(p => p.Id == requisicao.PastoId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(p => p.ProximaReforma, requisicao.DataPrevisao)
                    .SetProperty(p => p.Observacoes, requisicao.Observacoes)
                    .SetProperty(p => p.DataUltimaAtualizacao, timeProvider.GetUtcNow())
            );

        return new Sucesso(requisicao.PastoId);
    }

    private async Task<Resposta> AtualizarVigenteAsync(Vigente requisicao)
    {
        var erroMovimentacaoGados = await MoverGadosVinculadosAsync(requisicao);
        if (erroMovimentacaoGados is not null)
            return erroMovimentacaoGados;

        await contexto
            .Pastos.Where(p => p.Id == requisicao.PastoId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(p => p.Status, StatusPasto.EmReforma)
                    .SetProperty(p => p.Observacoes, requisicao.Observacoes)
                    .SetProperty(p => p.DataUltimaAtualizacao, timeProvider.GetUtcNow())
            );

        return new Sucesso(requisicao.PastoId);
    }

    private async Task<Erro?> MoverGadosVinculadosAsync(Vigente requisicao)
    {
        var loteDestinoExiste = await contexto.Lotes.AnyAsync(l =>
            l.Id == requisicao.LoteDestinoId
        );
        if (!loteDestinoExiste)
            return new Erro(TipoErro.LoteDestinoNaoEncontrado);

        var gadosParaMover = await contexto
            .Gados.Where(g => g.Lote!.PastoId == requisicao.PastoId && g.Status == StatusGado.Ativo)
            .Select(g => new
            {
                g.Id,
                LoteOrigemId = g.LoteId,
                g.PesoAtual,
            })
            .ToListAsync();

        if (gadosParaMover.Count > 0)
        {
            var agora = timeProvider.GetUtcNow();

            var movimentacoes = gadosParaMover.Select(g => new Movimentacao(
                gadoId: g.Id,
                loteOrigemId: g.LoteOrigemId,
                loteDestinoId: requisicao.LoteDestinoId,
                dataMovimentacao: agora,
                tipo: TipoMovimentacao.ReformaPasto,
                pesoNoMomento: g.PesoAtual,
                responsavelMovimentacao: "Sistema"
            ));

            await contexto.Movimentacoes.AddRangeAsync(movimentacoes);

            await contexto
                .Gados.Where(g => gadosParaMover.Select(g => g.Id).Contains(g.Id))
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(g => g.LoteId, requisicao.LoteDestinoId)
                        .SetProperty(g => g.DataUltimaAtualizacao, agora)
                );

            await contexto.SaveChangesAsync();
        }

        var aindaTemGadoVinculado = await contexto.Gados.AnyAsync(g =>
            g.Lote!.PastoId == requisicao.PastoId && g.Status == StatusGado.Ativo
        );

        if (aindaTemGadoVinculado)
            return new Erro(TipoErro.PastoAindaPossuiGado);

        return null;
    }

    private async Task<Resposta> AtualizarFinalizadaAsync(Finalizada requisicao)
    {
        var statusAtual = await contexto
            .Pastos.Where(p => p.Id == requisicao.PastoId)
            .Select(p => p.Status)
            .FirstOrDefaultAsync();

        if (statusAtual != StatusPasto.EmReforma)
            return TipoErro.StatusInvalidoParaFinalizar;

        var agora = timeProvider.GetUtcNow();

        await contexto
            .Pastos.Where(p => p.Id == requisicao.PastoId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(p => p.Status, StatusPasto.Ativo)
                    .SetProperty(p => p.UltimaReforma, agora)
                    .SetProperty(p => p.ProximaReforma, (DateTimeOffset?)null)
                    .SetProperty(p => p.Observacoes, requisicao.Observacoes)
                    .SetProperty(p => p.DataUltimaAtualizacao, agora)
            );

        return new Sucesso(requisicao.PastoId);
    }
}
