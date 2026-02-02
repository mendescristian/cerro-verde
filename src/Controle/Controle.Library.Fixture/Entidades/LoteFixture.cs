using System.Linq.Expressions;
using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Enums;
using Microsoft.EntityFrameworkCore;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Entidades;

public class LoteFixture
{
    private readonly ControleContexto _contexto;
    private readonly PastoFixture _pastoFixture;

    internal LoteFixture(ControleContexto contexto, PastoFixture pastoFixture)
    {
        _contexto = contexto;
        _pastoFixture = pastoFixture;
    }

    public static Lote CriarLote(
        int pastoId,
        int? numero = null,
        decimal? area = null,
        int? capacidadeMaxima = null,
        string? observacoes = null
    )
    {
        numero ??= FakeNumbers.CriarInt();
        area ??= FakeNumbers.CriarDecimal();
        capacidadeMaxima ??= FakeNumbers.CriarInt();

        return new Lote(pastoId, numero.Value, area.Value, capacidadeMaxima.Value, observacoes);
    }

    public async Task<Lote> CriarLoteAsync(Lote lote)
    {
        await _contexto.Lotes.AddAsync(lote);
        await _contexto.SaveChangesAsync();

        return lote;
    }

    /// <summary>
    /// Cria um Pasto no banco e em seguida cria um Lote vinculado a ele.
    /// </summary>
    public async Task<Lote> CriarLoteCompletoAsync(
        Pasto? pasto = null,
        int? numero = null,
        decimal? area = null,
        int? capacidadeMaxima = null,
        string? observacoes = null
    )
    {
        pasto ??= await _pastoFixture.CriarPastoCompletoAsync();

        var lote = CriarLote(
            pastoId: pasto.Id,
            numero: numero,
            area: area,
            capacidadeMaxima: capacidadeMaxima,
            observacoes: observacoes
        );

        return await CriarLoteAsync(lote);
    }

    public async Task<List<Lote>> ConsultarAsync(Expression<Func<Lote, bool>> predicado) =>
        await _contexto.Lotes.Where(predicado).ToListAsync();

    public async Task AtualizarStatusLoteAsync(
        Expression<Func<Lote, bool>> predicado,
        StatusLote status
    ) =>
        await _contexto
            .Lotes.Where(predicado)
            .ExecuteUpdateAsync(setter =>
                setter
                    .SetProperty(l => l.Status, status)
                    .SetProperty(l => l.DataUltimaAtualizacao, DateTimeOffset.UtcNow)
            );
}
