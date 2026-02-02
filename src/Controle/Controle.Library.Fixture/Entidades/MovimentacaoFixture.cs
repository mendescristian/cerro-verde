using System.Linq.Expressions;
using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Enums;
using Microsoft.EntityFrameworkCore;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Entidades;

public class MovimentacaoFixture
{
    private readonly ControleContexto _contexto;
    private readonly GadoFixture _gadoFixture;
    private readonly LoteFixture _loteFixture;

    internal MovimentacaoFixture(
        ControleContexto contexto,
        GadoFixture gadoFixture,
        LoteFixture loteFixture
    )
    {
        _contexto = contexto;
        _gadoFixture = gadoFixture;
        _loteFixture = loteFixture;
    }

    public static Movimentacao CriarMovimentacao(
        int gadoId,
        int loteOrigemId,
        int loteDestinoId,
        DateTimeOffset? dataMovimentacao = null,
        TipoMovimentacao? tipo = null,
        decimal? pesoNoMomento = null,
        string? responsavel = null
    )
    {
        dataMovimentacao ??= DateTimeOffset.UtcNow;
        tipo ??= TipoMovimentacao.Rotacao;
        pesoNoMomento ??= FakeNumbers.CriarDecimal();
        responsavel ??= "Peão Teste";

        return new Movimentacao(
            gadoId,
            loteOrigemId,
            loteDestinoId,
            dataMovimentacao.Value,
            tipo.Value,
            pesoNoMomento,
            responsavel
        );
    }

    public async Task<Movimentacao> CriarMovimentacaoAsync(Movimentacao movimentacao)
    {
        await _contexto.Movimentacoes.AddAsync(movimentacao);
        await _contexto.SaveChangesAsync();

        return movimentacao;
    }

    /// <summary>
    /// Cria um cenário completo: Gado (no lote origem) -> Movimentação -> Lote Destino.
    /// Se o Gado for passado, usa o Lote atual dele como origem (se loteOrigemId não for forçado).
    /// </summary>
    public async Task<Movimentacao> CriarMovimentacaoCompletaAsync(
        Gado? gado = null,
        Lote? loteDestino = null,
        TipoMovimentacao tipo = TipoMovimentacao.Rotacao
    )
    {
        gado ??= await _gadoFixture.CriarGadoCompletoAsync();

        var loteOrigemId = gado.LoteId;

        loteDestino ??= await _loteFixture.CriarLoteCompletoAsync();

        var movimentacao = CriarMovimentacao(
            gadoId: gado.Id,
            loteOrigemId: loteOrigemId,
            loteDestinoId: loteDestino.Id,
            tipo: tipo
        );

        return await CriarMovimentacaoAsync(movimentacao);
    }

    /// <summary>
    /// Simula uma troca de lote dentro do mesmo pasto.
    /// </summary>
    public async Task<Movimentacao> CriarRotacaoMesmoPastoAsync(Gado? gado = null)
    {
        gado ??= await _gadoFixture.CriarGadoCompletoAsync();

        var loteOrigem = await _loteFixture.ConsultarAsync(l => l.Id == gado.LoteId);

        var pastoId = loteOrigem.First().PastoId;

        var loteDestino = await _loteFixture.CriarLoteAsync(LoteFixture.CriarLote(pastoId));

        return await CriarMovimentacaoCompletaAsync(gado, loteDestino, TipoMovimentacao.Rotacao);
    }

    public async Task<List<Movimentacao>> ConsultarAsync(
        Expression<Func<Movimentacao, bool>> predicado
    ) =>
        await _contexto
            .Movimentacoes.Include(m => m.Gado)
            .Include(m => m.LoteOrigem)
            .Include(m => m.LoteDestino)
            .Where(predicado)
            .ToListAsync();
}
