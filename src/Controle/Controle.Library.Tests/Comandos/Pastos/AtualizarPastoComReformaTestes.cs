using Controle.Library.Comandos.Pastos;
using Controle.Library.Contexto;
using Controle.Library.Enums;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Models.Pastos;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Pastos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace Controle.Library.Tests.Comandos.Pastos;

public class AtualizarPastoComReformaTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<AtualizarPastoComReformaHandler> _logger = Substitute.For<
        ILogger<AtualizarPastoComReformaHandler>
    >();

    private readonly FakeTimeProvider _timeProvider = new(
        DateTimeOffset.Parse("2026-05-10T12:00:00Z")
    );

    [Test]
    public async Task Programada_QuandoPastoExiste_DeveAtualizarCorretamente()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        var requisicao = AtualizarPastoComReformaFixture.CriarProgramada(
            pastoId: pasto.Id,
            dataPrevisao: _timeProvider.GetUtcNow().AddDays(30)
        );

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<AtualizarPastoComReformaResposta.Sucesso>();

        Contexto.ChangeTracker.Clear();

        var pastoAtualizado = await Contexto
            .Pastos.AsNoTracking()
            .FirstAsync(p => p.Id == pasto.Id);

        pastoAtualizado.ProximaReforma.ShouldBe(requisicao.DataPrevisao);
        pastoAtualizado.Observacoes.ShouldBe(requisicao.Observacoes);

        pastoAtualizado.DataUltimaAtualizacao.ShouldBe(_timeProvider.GetUtcNow());
    }

    [Test]
    public async Task Programada_QuandoPastoNaoExiste_DeveRetornarErro()
    {
        // Given
        var requisicao = AtualizarPastoComReformaFixture.CriarProgramada(1);

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarPastoComReformaResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarPastoComReformaTipoErro.PastoNaoEncontrado);
    }

    [Test]
    public async Task Vigente_QuandoExistemGados_DeveMigrarMudarStatusEGerarMovimentacao()
    {
        // Given
        var loteOrigem = await Fixture.Lote.CriarLoteCompletoAsync();

        await Fixture.Gado.CriarLoteDeGadosAsync(5, loteOrigem);

        // 2. Setup do destino
        var loteDestino = await Fixture.Lote.CriarLoteCompletoAsync();

        var requisicao = AtualizarPastoComReformaFixture.CriarVigente(
            pastoId: loteOrigem.PastoId,
            loteDestinoId: loteDestino.Id,
            observacoes: "Iniciando obras"
        );

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<AtualizarPastoComReformaResposta.Sucesso>();

        Contexto.ChangeTracker.Clear();

        var pastoAtualizado = await Contexto
            .Pastos.AsNoTracking()
            .FirstAsync(p => p.Id == loteOrigem.PastoId);
        pastoAtualizado.Status.ShouldBe(StatusPasto.EmReforma);
        pastoAtualizado.Observacoes.ShouldBe("Iniciando obras");
        pastoAtualizado.DataUltimaAtualizacao.ShouldBe(_timeProvider.GetUtcNow());

        var gadosNoDestino = await Contexto
            .Gados.AsNoTracking()
            .Where(g => g.LoteId == loteDestino.Id)
            .ToListAsync();

        gadosNoDestino.Count.ShouldBe(5);
        gadosNoDestino
            .All(g => g.DataUltimaAtualizacao == _timeProvider.GetUtcNow())
            .ShouldBeTrue();

        var movimentacoes = await Contexto
            .Movimentacoes.AsNoTracking()
            .Where(m => m.Tipo == TipoMovimentacao.ReformaPasto)
            .ToListAsync();

        movimentacoes.Count.ShouldBe(5);
        movimentacoes.All(m => m.LoteDestinoId == loteDestino.Id).ShouldBeTrue();
        movimentacoes.All(m => m.DataMovimentacao == _timeProvider.GetUtcNow()).ShouldBeTrue();
    }

    [Test]
    public async Task Vigente_QuandoLoteDestinoNaoExiste_DeveRetornarErro()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();

        var requisicao = AtualizarPastoComReformaFixture.CriarVigente(
            pastoId: pasto.Id,
            loteDestinoId: 9999
        );

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarPastoComReformaResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarPastoComReformaTipoErro.LoteDestinoNaoEncontrado);

        Contexto.ChangeTracker.Clear();

        var pastoNoBanco = await Contexto.Pastos.AsNoTracking().FirstAsync(p => p.Id == pasto.Id);
        pastoNoBanco.Status.ShouldNotBe(StatusPasto.EmReforma);
    }

    [Test]
    public async Task Finalizada_QuandoStatusCorreto_DeveAtualizarCorretamente()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();
        await Fixture.Pasto.AtualizarStatusPastoAsync(p => p.Id == pasto.Id, StatusPasto.EmReforma);

        var requisicao = AtualizarPastoComReformaFixture.CriarFinalizada(
            pastoId: pasto.Id,
            observacoes: "Reforma Finalizada com pendencias"
        );

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<AtualizarPastoComReformaResposta.Sucesso>();

        Contexto.ChangeTracker.Clear();

        var pastoAtualizado = await Contexto
            .Pastos.AsNoTracking()
            .FirstAsync(p => p.Id == pasto.Id);
        pastoAtualizado.Status.ShouldBe(StatusPasto.Ativo);
        pastoAtualizado.ProximaReforma.ShouldBeNull();
        pastoAtualizado.Observacoes.ShouldBe(requisicao.Observacoes);
        pastoAtualizado.UltimaReforma.ShouldBe(_timeProvider.GetUtcNow());
        pastoAtualizado.DataUltimaAtualizacao.ShouldBe(_timeProvider.GetUtcNow());
    }

    [Test]
    public async Task Finalizada_QuandoStatusInvalido_DeveRetornarErro()
    {
        // Given
        var pasto = await Fixture.Pasto.CriarPastoCompletoAsync();
        var requisicao = AtualizarPastoComReformaFixture.CriarFinalizada(pastoId: pasto.Id);

        var handler = new AtualizarPastoComReformaHandler(Contexto, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarPastoComReformaResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarPastoComReformaTipoErro.StatusInvalidoParaFinalizar);
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveRetornarErroInterno()
    {
        // Given
        var requisicao = AtualizarPastoComReformaFixture.CriarProgramada(1);

        var handler = new AtualizarPastoComReformaHandler(null!, _logger, _timeProvider);
        var comando = new AtualizarPastoComReformaComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarPastoComReformaResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarPastoComReformaTipoErro.ErroInterno);
    }
}
