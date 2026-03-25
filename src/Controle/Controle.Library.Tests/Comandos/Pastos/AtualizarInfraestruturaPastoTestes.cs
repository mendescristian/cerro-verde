using Controle.Library.Comandos.Pastos;
using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Models.Pastos;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Pastos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Controle.Library.Tests.Comandos.Pastos;

public class AtualizarInfraestruturaPastoTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<AtualizarInfraestruturaPastoHandler> _logger = Substitute.For<
        ILogger<AtualizarInfraestruturaPastoHandler>
    >();

    [Test]
    public async Task QuandoPastoNaoExistir_DeveRetornarErroNaoEncontrado()
    {
        // Given
        var requisicao = AtualizarInfraestruturaPastoFixture.CriarRequisicao(1);

        var handler = new AtualizarInfraestruturaPastoHandler(Contexto, _logger);

        var comando = new AtualizarInfraestruturaPastoComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarInfraestruturaPastoResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarInfraestruturaPastoTipoErro.PastoNaoEncontrado);
    }

    [Test]
    public async Task QuandoPastoExistir_DeveAtualizarInfraestruturaCorretamente()
    {
        // Given
        var pastoOriginal = await Fixture.Pasto.CriarPastoCompletoAsync();

        var requisicao = AtualizarInfraestruturaPastoFixture.CriarRequisicao(
            pastoId: pastoOriginal.Id,
            area: 5000m,
            temSombra: false,
            observacoes: "A grama do meu vizinho não é mais verde que a minha"
        );

        var handler = new AtualizarInfraestruturaPastoHandler(Contexto, _logger);
        var comando = new AtualizarInfraestruturaPastoComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<AtualizarInfraestruturaPastoResposta.Sucesso>();

        Contexto.ChangeTracker.Clear();

        var pastoAtualizado = await Contexto
            .Pastos.AsNoTracking()
            .FirstAsync(p => p.Id == pastoOriginal.Id);

        pastoAtualizado.Area.ShouldBe(requisicao.Area);
        pastoAtualizado.CapacidadeMaxima.ShouldBe(requisicao.CapacidadeMaxima);
        pastoAtualizado.TemSombra.ShouldBeFalse();
        pastoAtualizado.Observacoes.ShouldBe(requisicao.Observacoes);
        pastoAtualizado.TemAguadouro.ShouldBeTrue();
        pastoAtualizado
            .DadosAguadouro.ShouldNotBeNull()
            .ShouldBeEquivalentTo(requisicao.DadosAguadouro);
        pastoAtualizado.TemCochoSal.ShouldBeTrue();
        pastoAtualizado
            .DadosCochoSal.ShouldNotBeNull()
            .ShouldBeEquivalentTo(requisicao.DadosCochoSal);
        pastoAtualizado.DataUltimaAtualizacao.ShouldNotBeNull();
    }

    [Test]
    public async Task QuandoRemoverInfraestrutura_DeveMarcarFlagsComoFalse()
    {
        // Given
        var pastoOriginal = await Fixture.Pasto.CriarPastoCompletoAsync();

        var requisicao = AtualizarInfraestruturaPastoFixture.CriarRequisicao(
            pastoId: pastoOriginal.Id,
            gerarInfraestrutura: false
        );

        var handler = new AtualizarInfraestruturaPastoHandler(Contexto, _logger);
        var comando = new AtualizarInfraestruturaPastoComando(requisicao);

        // When
        await handler.Handle(comando, default);

        // Then
        Contexto.ChangeTracker.Clear();

        var pasto = await Contexto.Pastos.AsNoTracking().FirstAsync(p => p.Id == pastoOriginal.Id);

        pasto.TemAguadouro.ShouldBeFalse();
        pasto.DadosAguadouro.ShouldBeNull();

        pasto.TemCochoSal.ShouldBeFalse();
        pasto.DadosCochoSal.ShouldBeNull();
    }

    [Test]
    public async Task QuandoOcorrerExcecao_DeveLogarERetornarErroInterno()
    {
        // Given
        var requisicao = AtualizarInfraestruturaPastoFixture.CriarRequisicao();

        var handler = new AtualizarInfraestruturaPastoHandler(null!, _logger);
        var comando = new AtualizarInfraestruturaPastoComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta
            .ShouldBeOfType<AtualizarInfraestruturaPastoResposta.Erro>()
            .TipoErro.ShouldBe(AtualizarInfraestruturaPastoTipoErro.ErroInterno);
    }
}
