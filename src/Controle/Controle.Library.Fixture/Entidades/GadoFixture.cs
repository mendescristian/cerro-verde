using System.Linq.Expressions;
using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Enums;
using Microsoft.EntityFrameworkCore;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Entidades;

public class GadoFixture
{
    private readonly ControleContexto _contexto;
    private readonly LoteFixture _loteFixture;

    internal GadoFixture(ControleContexto contexto, LoteFixture loteFixture)
    {
        _contexto = contexto;
        _loteFixture = loteFixture;
    }

    public static Gado CriarGado(
        int loteId,
        int? brinco = null,
        SexoGado? sexo = null,
        RacaGado? raca = null,
        DateTimeOffset? dataNascimento = null,
        decimal? pesoAtual = null,
        int? paiId = null,
        int? maeId = null,
        OrigemGado? origem = null,
        GadoDadosEntrada? dadosEntrada = null,
        FinalidadeGado? finalidade = null,
        string? observacoes = null
    )
    {
        brinco ??= FakeNumbers.CriarInt();
        sexo ??= SexoGado.Macho;
        raca ??= RacaGado.Nelore;
        dataNascimento ??= DateTimeOffset.UtcNow.AddMonths(-24);
        pesoAtual ??= FakeNumbers.CriarDecimal();
        origem ??= OrigemGado.NascidoNaFazenda;
        finalidade ??= FinalidadeGado.Corte;

        return new Gado(
            loteId,
            brinco.Value,
            sexo.Value,
            raca.Value,
            dataNascimento.Value,
            pesoAtual.Value,
            paiId,
            maeId,
            origem.Value,
            dadosEntrada,
            finalidade.Value,
            observacoes
        );
    }

    public static GadoDadosEntrada CriarDadosEntrada(
        string? origem = null,
        long? valorCompra = null,
        DateTimeOffset? data = null
    ) =>
        new(
            origem ?? "Fazenda Fornecedora Teste",
            valorCompra ?? FakeNumbers.CriarLong(),
            data ?? DateTimeOffset.UtcNow.AddMonths(-1)
        );

    public static GadoDadosSaida CriarDadosSaida(
        string? motivo = null,
        DateTimeOffset? data = null
    ) => new(motivo ?? "Venda para Frigorífico", data ?? DateTimeOffset.UtcNow);

    public static GadoDadosPrenhez CriarDadosPrenhez(
        OrigemPrenhez? origem = null,
        DateTimeOffset? dataPrevisaoParto = null
    ) =>
        new(
            origem ?? OrigemPrenhez.Inseminacao,
            dataPrevisaoParto ?? DateTimeOffset.UtcNow.AddMonths(9)
        );

    public async Task<Gado> CriarGadoAsync(Gado gado)
    {
        await _contexto.Gados.AddAsync(gado);
        await _contexto.SaveChangesAsync();

        return gado;
    }

    /// <summary>
    /// Cria um Gado garantindo que ele esteja vinculado a um Lote (e consequentemente a um Pasto) válido no banco.
    /// </summary>
    public async Task<Gado> CriarGadoCompletoAsync(
        Lote? lote = null,
        int? brinco = null,
        SexoGado? sexo = null,
        GadoDadosEntrada? dadosEntrada = null
    )
    {
        lote ??= await _loteFixture.CriarLoteCompletoAsync();

        var gado = CriarGado(
            loteId: lote.Id,
            brinco: brinco,
            sexo: sexo,
            dadosEntrada: dadosEntrada
        );

        return await CriarGadoAsync(gado);
    }

    /// <summary>
    /// Cria uma lista de gados vinculados ao mesmo lote.
    /// </summary>
    public async Task<List<Gado>> CriarLoteDeGadosAsync(int quantidade, Lote? lote = null)
    {
        lote ??= await _loteFixture.CriarLoteCompletoAsync();

        var gados = new List<Gado>();

        for (int i = 0; i < quantidade; i++)
            gados.Add(CriarGado(loteId: lote.Id));

        await _contexto.Gados.AddRangeAsync(gados);
        await _contexto.SaveChangesAsync();

        return gados;
    }

    public async Task<List<Gado>> ConsultarAsync(Expression<Func<Gado, bool>> predicado) =>
        await _contexto.Gados.Where(predicado).ToListAsync();

    public async Task AtualizarStatusGadoAsync(
        Expression<Func<Gado, bool>> predicado,
        StatusGado status
    ) =>
        await _contexto
            .Gados.Where(predicado)
            .ExecuteUpdateAsync(setter =>
                setter
                    .SetProperty(g => g.Status, status)
                    .SetProperty(g => g.DataUltimaAtualizacao, DateTimeOffset.UtcNow)
            );

    public async Task AtualizarGadoComoVendidoAsync(int gadoId, GadoDadosSaida dadosSaida)
    {
        await _contexto
            .Gados.Where(g => g.Id == gadoId)
            .ExecuteUpdateAsync(setter =>
                setter
                    .SetProperty(g => g.Status, StatusGado.Vendido)
                    .SetProperty(g => g.Valido, false)
                    .SetProperty(g => g.DadosSaida, dadosSaida)
                    .SetProperty(g => g.DataUltimaAtualizacao, DateTimeOffset.UtcNow)
            );
    }
}
