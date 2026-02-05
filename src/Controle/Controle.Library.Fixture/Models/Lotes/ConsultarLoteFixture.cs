using Controle.Library.Enums;
using Controle.Library.Models.Lotes;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Models.Lotes;

public static class ConsultarLoteFixture
{
    public static ConsultarLoteResposta CriarResposta(
        int? id = null,
        int? pastoId = null,
        int? numero = null,
        decimal? area = null,
        int? capacidadeMaxima = null,
        StatusLote? status = null,
        string? observacoes = null,
        DateTimeOffset? dataInclusao = null
    ) =>
        new(
            id ?? FakeNumbers.CriarInt(),
            pastoId ?? FakeNumbers.CriarInt(),
            numero ?? FakeNumbers.CriarInt(),
            area ?? FakeNumbers.CriarDecimal(),
            capacidadeMaxima ?? FakeNumbers.CriarInt(),
            status ?? StatusLote.Ocupado,
            observacoes ?? "Observação",
            dataInclusao ?? DateTimeOffset.UtcNow
        );
}
