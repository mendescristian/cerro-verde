using Controle.Library.Comandos.Pastos;
using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Pastos;
using Microsoft.Extensions.Logging;

namespace Controle.Library.Tests.Comandos.Pastos;

public class ExcluirPastoTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<ExcluirPastoHandler> _logger = Substitute.For<
        ILogger<ExcluirPastoHandler>
    >();

    [Test]
    public async Task QuandoPastoEstiverVazio_DeveExcluirPastoELotes()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        await Fixture.Lote.CriarLoteCompletoAsync(pasto);
        await Fixture.Lote.CriarLoteCompletoAsync(pasto);

        var handler = new ExcluirPastoHandler(Contexto, _logger);
        var comando = new ExcluirPastoComando(pasto.Id);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<ExcluirPastoResposta.Sucesso>();

        Contexto.Pastos.ShouldBeEmpty();
        Contexto.Lotes.ShouldBeEmpty();
    }

    [Test]
    public async Task QuandoPastoPossuirGadoAtivo_DeveRetornarErroDeBloqueio()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();
        var lote = await Fixture.Lote.CriarLoteCompletoAsync(pasto);

        await Fixture.Gado.CriarLoteDeGadosAsync(1, lote);

        var handler = new ExcluirPastoHandler(Contexto, _logger);
        var comando = new ExcluirPastoComando(pasto.Id);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirPastoResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirPastoTipoErro.PastoPossuiGadoVinculado);

        Contexto.Pastos.ShouldHaveSingleItem();
        Contexto.Lotes.ShouldHaveSingleItem();
    }

    [Test]
    public async Task QuandoPastoNaoExistir_DeveRetornarErroNaoEncontrado()
    {
        // Given
        var handler = new ExcluirPastoHandler(Contexto, _logger);
        var comando = new ExcluirPastoComando(1);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirPastoResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirPastoTipoErro.PastoNaoEncontrado);
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveRetornarErroInterno()
    {
        // Given
        var handler = new ExcluirPastoHandler(null!, _logger);
        var comando = new ExcluirPastoComando(1);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirPastoResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirPastoTipoErro.ErroInterno);
    }
}
