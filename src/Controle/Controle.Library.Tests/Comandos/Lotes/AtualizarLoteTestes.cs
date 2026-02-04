using Controle.Library.Comandos.Lotes;
using Controle.Library.Contexto;
using Controle.Library.Enums;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Models.Lotes;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Lotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace Controle.Library.Tests.Comandos.Lotes;

public class AtualizarLoteTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<AtualizarLoteHandler> _logger = Substitute.For<
        ILogger<AtualizarLoteHandler>
    >();

    private readonly FakeTimeProvider _timeProvider = new(
        DateTimeOffset.Parse("2026-06-15T10:00:00Z")
    );

    [Test]
    public async Task QuandoLoteExistir_DeveAtualizarCamposCorretamente()
    {
        // Given
        var loteOriginal = await Fixture.Lote.CriarLoteCompletoAsync();

        var requisicao = AtualizarLoteFixture.CriarRequisicao(
            loteId: loteOriginal.Id,
            area: 123.45m,
            capacidadeMaxima: 500,
            status: StatusLote.Manutencao,
            observacoes: "Cerca elétrica instalada"
        );

        var handler = new AtualizarLoteHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<AtualizarLoteResposta.Sucesso>();

        Contexto.ChangeTracker.Clear();

        var loteAtualizado = await Contexto.Lotes.FirstAsync(l => l.Id == loteOriginal.Id);

        loteAtualizado.Area.ShouldBe(requisicao.Area);
        loteAtualizado.CapacidadeMaxima.ShouldBe(requisicao.CapacidadeMaxima);
        loteAtualizado.Status.ShouldBe(requisicao.Status);
        loteAtualizado.Observacoes.ShouldBe(requisicao.Observacoes);
        loteAtualizado.DataUltimaAtualizacao.ShouldBe(_timeProvider.GetUtcNow());
        loteAtualizado.Numero.ShouldBe(loteOriginal.Numero);
    }

    [Test]
    public async Task QuandoLoteNaoExistir_DeveRetornarErroNaoEncontrado()
    {
        // Given
        var requisicao = AtualizarLoteFixture.CriarRequisicao(loteId: 9999);
        var handler = new AtualizarLoteHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarLoteResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarLoteTipoErro.LoteNaoEncontrado);
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveRetornarErroInterno()
    {
        // Given
        var requisicao = AtualizarLoteFixture.CriarRequisicao(1);

        var handler = new AtualizarLoteHandler(null!, _logger, _timeProvider);

        var comando = new AtualizarLoteComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarLoteResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarLoteTipoErro.ErroInterno);
    }
}
