using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Providers;
using Controle.Library.Queries.Lotes;

namespace Controle.Library.Tests.Queries.Lotes;

public class ConsultarLotePorIdTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    [Test]
    public async Task QuandoLoteExistir_DeveRetornarCorretamente()
    {
        // Given
        var lote = await Fixture.Lote.CriarLoteCompletoAsync();

        var query = new ConsultarLotePorIdQuery(lote.Id);
        var handler = new ConsultarLotePorIdHandler(Contexto);

        // When
        var resposta = await handler.Handle(query, default);

        // Then
        resposta.ShouldNotBeNull();
        resposta.Id.ShouldBe(lote.Id);
        resposta.PastoId.ShouldBe(lote.PastoId);
        resposta.Numero.ShouldBe(lote.Numero);
        resposta.Area.ShouldBe(lote.Area);
        resposta.Status.ShouldBe(lote.Status);
        resposta.Observacoes.ShouldBe(lote.Observacoes);
        resposta.DataInclusao.ShouldBe(lote.DataInclusao);
    }

    [Test]
    public async Task QuandoLoteNaoExistir_DeveRetornarNull()
    {
        // Given
        var query = new ConsultarLotePorIdQuery(9999);
        var handler = new ConsultarLotePorIdHandler(Contexto);

        // When
        var resposta = await handler.Handle(query, default);

        // Then
        resposta.ShouldBeNull();
    }
}
