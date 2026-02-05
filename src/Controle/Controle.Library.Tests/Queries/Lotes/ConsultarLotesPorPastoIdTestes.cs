using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Providers;
using Controle.Library.Queries.Lotes;

namespace Controle.Library.Tests.Queries.Lotes;

public class ConsultarLotesPorPastoIdTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    [Test]
    public async Task QuandoPastoTiverLotes_DeveRetornarLotesCorretamente()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        var lote1 = await Fixture.Lote.CriarLoteCompletoAsync(pasto, numero: 1);
        var lote2 = await Fixture.Lote.CriarLoteCompletoAsync(pasto, numero: 2);

        var loteDeOutroPasto = await Fixture.Lote.CriarLoteCompletoAsync(numero: 3);

        var query = new ConsultarLotesPorPastoIdQuery(pasto.Id);
        var handler = new ConsultarLotesPorPastoIdHandler(Contexto);

        // When
        var resposta = await handler.Handle(query, default);

        // Then
        resposta.ShouldNotBeNull().Count.ShouldBe(2);

        resposta.ShouldContain(l => l.Id == lote1.Id);
        resposta.ShouldContain(l => l.Id == lote2.Id);
        resposta.ShouldNotContain(l => l.PastoId == loteDeOutroPasto.PastoId);
    }

    [Test]
    public async Task QuandoPastoNaoTiverLotes_DeveRetornarListaVazia()
    {
        // Given
        var pastoVazio = await Fixture.Pasto.CriarPastoCompletoAsync();

        var query = new ConsultarLotesPorPastoIdQuery(pastoVazio.Id);
        var handler = new ConsultarLotesPorPastoIdHandler(Contexto);

        // When
        var resposta = await handler.Handle(query, default);

        // Then
        resposta.ShouldNotBeNull().ShouldBeEmpty();
    }

    [Test]
    public async Task QuandoPastoNaoExistir_DeveRetornarListaVazia()
    {
        // Given
        var query = new ConsultarLotesPorPastoIdQuery(9999);

        var handler = new ConsultarLotesPorPastoIdHandler(Contexto);

        // When
        var resposta = await handler.Handle(query, default);

        // Then
        resposta.ShouldNotBeNull().ShouldBeEmpty();
    }
}
