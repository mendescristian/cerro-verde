using System.Linq.Expressions;
using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Enums;
using Microsoft.EntityFrameworkCore;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Entidades;

public class PastoFixture
{
    private readonly ControleContexto _contexto;

    internal PastoFixture(ControleContexto contexto)
    {
        _contexto = contexto;
    }

    public static Pasto CriarPasto(
        string? nome = null,
        decimal? area = null,
        TipoPasto? tipoPasto = null,
        int? capacidadeMaxima = null,
        DadosAguadouroPasto? dadosAguadouro = null,
        DadosCochoSalPasto? dadosCochoSal = null,
        bool? temSombra = null,
        string? observacoes = null
    )
    {
        nome ??= $"Pasto {FakeNumbers.CriarInt()}-{Guid.NewGuid().ToString()[..8]}";
        area ??= FakeNumbers.CriarDecimal();
        tipoPasto ??= TipoPasto.Brachiaria;
        capacidadeMaxima ??= FakeNumbers.CriarInt();
        temSombra ??= false;

        return new Pasto(
            nome,
            area.Value,
            tipoPasto.Value,
            capacidadeMaxima.Value,
            dadosAguadouro,
            dadosCochoSal,
            temSombra.Value,
            observacoes
        );
    }

    public static DadosAguadouroPasto CriarDadosAguadouro(
        DateTimeOffset? dataUltimaAfericao = null,
        decimal? quantidadeAferida = null,
        string? responsavel = null
    )
    {
        return new DadosAguadouroPasto(
            dataUltimaAfericao ?? DateTimeOffset.UtcNow,
            quantidadeAferida ?? FakeNumbers.CriarDecimal(),
            responsavel ?? "Responsável Teste"
        );
    }

    public static DadosCochoSalPasto CriarDadosCochoSal(
        DateTimeOffset? dataUltimaAfericao = null,
        decimal? quantidadeAferida = null,
        string? responsavel = null
    )
    {
        return new DadosCochoSalPasto(
            dataUltimaAfericao ?? DateTimeOffset.UtcNow,
            quantidadeAferida ?? FakeNumbers.CriarDecimal(),
            responsavel ?? "Responsável Teste"
        );
    }

    public async Task<Pasto> CriarPastoAsync(Pasto? pasto = null)
    {
        pasto ??= CriarPasto();

        await _contexto.Pastos.AddAsync(pasto);
        await _contexto.SaveChangesAsync();

        return pasto;
    }

    /// <summary>
    /// Cria um pasto completo com dados de infraestrutura (Aguadouro e Cocho)
    /// </summary>
    public async Task<Pasto> CriarPastoCompletoAsync()
    {
        var pasto = CriarPasto(
            dadosAguadouro: CriarDadosAguadouro(),
            dadosCochoSal: CriarDadosCochoSal(),
            temSombra: true,
            observacoes: "Pasto completo gerado via fixture"
        );

        return await CriarPastoAsync(pasto);
    }

    public async Task<List<Pasto>> ConsultarAsync(Expression<Func<Pasto, bool>> predicado) =>
        await _contexto.Pastos.Where(predicado).ToListAsync();

    public async Task AtualizarStatusPastoAsync(
        Expression<Func<Pasto, bool>> predicado,
        StatusPasto status
    ) =>
        await _contexto
            .Pastos.Where(predicado)
            .ExecuteUpdateAsync(setter =>
                setter
                    .SetProperty(p => p.Status, status)
                    .SetProperty(p => p.DataUltimaAtualizacao, DateTimeOffset.UtcNow)
            );
}
