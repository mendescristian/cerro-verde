using Controle.Library.Comandos.Lotes;
using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Models.Lotes;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Lotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Controle.Library.Tests.Comandos.Lotes;

public class CadastrarLoteTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<CadastrarLoteHandler> _logger = Substitute.For<
        ILogger<CadastrarLoteHandler>
    >();

    [Test]
    public async Task QuandoPastoNaoExistir_DeveRetornarErro()
    {
        // Given
        var requisicao = CadastrarLoteFixture.CriarRequisicao(pastoId: 9999);
        var handler = new CadastrarLoteHandler(Contexto, _logger);
        var comando = new CadastrarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<CadastrarLoteResposta.Erro>()
            .TipoErro.ShouldBe(CadastrarLoteTipoErro.PastoNaoEncontrado);
    }

    [Test]
    public async Task QuandoForPrimeiroLoteDoPasto_DeveCadastrarComNumeroUm()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        var requisicao = CadastrarLoteFixture.CriarRequisicao(
            pastoId: pasto.Id,
            area: 50m,
            capacidadeMaxima: 10
        );

        var handler = new CadastrarLoteHandler(Contexto, _logger);
        var comando = new CadastrarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        var sucesso = resposta.ShouldBeOfType<CadastrarLoteResposta.Sucesso>();

        var loteNoBanco = await Contexto.Lotes.FirstAsync(l => l.Id == sucesso.LoteId);
        loteNoBanco.PastoId.ShouldBe(pasto.Id);
        loteNoBanco.Numero.ShouldBe(1);
        loteNoBanco.Area.ShouldBe(requisicao.Area);
        loteNoBanco.CapacidadeMaxima.ShouldBe(requisicao.CapacidadeMaxima);
    }

    [Test]
    public async Task QuandoJaExistiremLotes_DeveCadastrarComProximoNumero()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        await Fixture.Lote.CriarLoteCompletoAsync(pasto, numero: 1);
        await Fixture.Lote.CriarLoteCompletoAsync(pasto, numero: 5);

        var requisicao = CadastrarLoteFixture.CriarRequisicao(pastoId: pasto.Id);

        var handler = new CadastrarLoteHandler(Contexto, _logger);
        var comando = new CadastrarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        var sucesso = resposta.ShouldBeOfType<CadastrarLoteResposta.Sucesso>();

        var loteNoBanco = await Contexto.Lotes.FirstAsync(l => l.Id == sucesso.LoteId);
        loteNoBanco.Numero.ShouldBe(6);
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveRetornarErroInterno()
    {
        // Given
        var requisicao = CadastrarLoteFixture.CriarRequisicao(1);

        var handler = new CadastrarLoteHandler(null!, _logger);
        var comando = new CadastrarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<CadastrarLoteResposta.Erro>()
            .TipoErro.ShouldBe(CadastrarLoteTipoErro.ErroInterno);
    }
}
