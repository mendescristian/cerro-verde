using Controle.Library.Comandos.Pastos;
using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Controle.Library.Fixture.Entidades;
using Controle.Library.Fixture.Models.Pastos;
using Controle.Library.Fixture.Providers;
using Controle.Library.Models.Pastos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Controle.Library.Tests.Comandos.Pastos;

public class CadastrarPastoTestes
{
    [ClassDataSource<ControleContextoProvider>(Shared = SharedType.None)]
    public required ControleContextoProvider ContextoProvider { get; init; }

    private ControleContexto Contexto => ContextoProvider.Contexto;
    private ControleContextoFixture Fixture => ContextoProvider.Fixture;

    private readonly ILogger<CadastrarPastoHandler> _logger = Substitute.For<
        ILogger<CadastrarPastoHandler>
    >();

    [Test]
    public async Task QuandoNomeJaExistir_DeveRetornarErro()
    {
        // Given
        const string nomeDuplicado = "Pasto Norte";

        await Fixture.Pasto.CriarPastoAsync(PastoFixture.CriarPasto(nome: nomeDuplicado));

        var requisicao = CadastrarPastoFixture.CriarRequisicao(nome: nomeDuplicado);

        var handler = new CadastrarPastoHandler(Contexto, _logger);
        var comando = new CadastrarPastoComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        resposta.ShouldBeOfType<CadastrarPastoResposta.Erro>();

        var respostaErro = (CadastrarPastoResposta.Erro)resposta;
        respostaErro.TipoErro.ShouldBe(CadastrarPastoTipoErro.NomeJaCadastrado);

        Contexto.Pastos.ShouldHaveSingleItem();
    }

    [Test]
    public async Task QuandoDadosValidos_DeveCadastrarComSucesso()
    {
        // Given
        var requisicao = CadastrarPastoFixture.CriarRequisicao(nome: "Pasto Sul");

        var handler = new CadastrarPastoHandler(Contexto, _logger);
        var comando = new CadastrarPastoComando(requisicao);

        // When
        var resposta = await handler.Handle(comando, default);

        // Then
        var sucesso = resposta.ShouldBeOfType<CadastrarPastoResposta.Sucesso>();

        var pastoInserido = await Contexto.Pastos.FirstAsync(p => p.Id == sucesso.PastoId);
        pastoInserido.Nome.ShouldBe(requisicao.Nome);
        pastoInserido.Area.ShouldBe(requisicao.Area);
        pastoInserido.TipoPasto.ShouldBe(requisicao.TipoPasto);
        pastoInserido.CapacidadeMaxima.ShouldBe(requisicao.CapacidadeMaxima);
        pastoInserido.TemAguadouro.ShouldBeTrue();
        pastoInserido.DadosAguadouro.ShouldBeEquivalentTo(requisicao.DadosAguadouro);
        pastoInserido.TemCochoSal.ShouldBeTrue();
        pastoInserido.DadosCochoSal.ShouldBeEquivalentTo(requisicao.DadosCochoSal);
        pastoInserido.TemSombra.ShouldBe(requisicao.TemSombra);
        pastoInserido.Observacoes.ShouldBe(requisicao.Observacoes);
    }
}
