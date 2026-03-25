using Controle.Library.Models.Lotes;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Models.Lotes;

public static class CadastrarLoteFixture
{
    public static CadastrarLoteRequisicao CriarRequisicao(
        int pastoId,
        decimal? area = null,
        int? capacidadeMaxima = null,
        string? observacoes = null
    ) =>
        new(
            pastoId,
            area ?? FakeNumbers.CriarDecimal(),
            capacidadeMaxima ?? FakeNumbers.CriarInt(),
            observacoes ?? "Novo Lote Teste"
        );
}
