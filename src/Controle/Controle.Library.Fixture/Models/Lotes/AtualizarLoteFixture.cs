using Controle.Library.Enums;
using Controle.Library.Models.Lotes;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Models.Lotes;

public static class AtualizarLoteFixture
{
    public static AtualizarLoteRequisicao CriarRequisicao(
        int loteId,
        decimal? area = null,
        int? capacidadeMaxima = null,
        StatusLote? status = null,
        string? observacoes = null
    ) =>
        new(
            loteId,
            area ?? FakeNumbers.CriarDecimal(),
            capacidadeMaxima ?? FakeNumbers.CriarInt(),
            status ?? StatusLote.Ocupado,
            observacoes ?? "Lote Atualizado Teste"
        );
}
