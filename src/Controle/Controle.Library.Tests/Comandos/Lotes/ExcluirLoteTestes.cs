using Controle.Library.Comandos.Lotes;
using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Lotes;
using Microsoft.Extensions.Logging;

namespace Controle.Library.Tests.Comandos.Lotes;

public class ExcluirLoteTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<ExcluirLoteHandler> _logger = Substitute.For<
        ILogger<ExcluirLoteHandler>
    >();

    [Test]
    public async Task QuandoLoteEstiverVazio_DeveExcluirComSucesso()
    {
        // Given
        var lote = await Fixture.Lote.CriarLoteCompletoAsync();

        var handler = new ExcluirLoteHandler(Contexto, _logger);
        var comando = new ExcluirLoteComando(lote.Id);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<ExcluirLoteResposta.Sucesso>();

        Contexto.Lotes.ShouldBeEmpty();
    }

    [Test]
    public async Task QuandoLotePossuirGadoAtivo_DeveRetornarErroDeBloqueio()
    {
        // Given
        var lote = await Fixture.Lote.CriarLoteCompletoAsync();

        await Fixture.Gado.CriarLoteDeGadosAsync(1, lote);

        var handler = new ExcluirLoteHandler(Contexto, _logger);
        var comando = new ExcluirLoteComando(lote.Id);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirLoteResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirLoteTipoErro.LotePossuiGadoVinculado);

        Contexto.Lotes.ShouldHaveSingleItem();
    }

    [Test]
    public async Task QuandoGadoEstiverInativo_DevePermitirExclusao()
    {
        // Given
        var lote = await Fixture.Lote.CriarLoteCompletoAsync();
        var gado = await Fixture.Gado.CriarGadoCompletoAsync(lote);

        await Fixture.Gado.AtualizarGadoComoVendidoAsync(
            gado.Id,
            new GadoDadosSaida("Vendi caro", DateTimeOffset.UtcNow)
        );

        var handler = new ExcluirLoteHandler(Contexto, _logger);

        var comando = new ExcluirLoteComando(lote.Id);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<ExcluirLoteResposta.Sucesso>();

        Contexto.Lotes.ShouldBeEmpty();
    }

    [Test]
    public async Task QuandoLoteNaoExistir_DeveRetornarErroNaoEncontrado()
    {
        // Given
        var handler = new ExcluirLoteHandler(Contexto, _logger);
        var comando = new ExcluirLoteComando(1);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirLoteResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirLoteTipoErro.LoteNaoEncontrado);
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveRetornarErroInterno()
    {
        // Given
        var handler = new ExcluirLoteHandler(null!, _logger);
        var comando = new ExcluirLoteComando(1);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<ExcluirLoteResposta.Erro>()
            .TipoErro.ShouldBe(ExcluirLoteTipoErro.ErroInterno);
    }
}
