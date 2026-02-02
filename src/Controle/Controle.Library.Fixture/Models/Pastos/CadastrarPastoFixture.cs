using Controle.Library.Entidades;
using Controle.Library.Enums;
using Controle.Library.Fixture.Entidades;
using Controle.Library.Models.Pastos;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Models.Pastos;

public static class CadastrarPastoFixture
{
    public static CadastrarPastoRequisicao CriarRequisicao(
        string? nome = null,
        decimal? area = null,
        TipoPasto? tipoPasto = null,
        int? capacidadeMaxima = null,
        DadosAguadouroPasto? dadosAguadouro = null,
        DadosCochoSalPasto? dadosCochoSal = null,
        bool? temSombra = null,
        string? observacoes = null,
        bool gerarInfraestrutura = true
    )
    {
        nome ??= $"Pasto {FakeNumbers.CriarInt()}-{Guid.NewGuid().ToString()[..5]}";
        area ??= FakeNumbers.CriarDecimal();
        tipoPasto ??= TipoPasto.Brachiaria;
        capacidadeMaxima ??= FakeNumbers.CriarInt();
        temSombra ??= true;
        observacoes ??= "A grama do meu vizinho não é mais verde que a minha";

        if (gerarInfraestrutura)
        {
            dadosAguadouro ??= PastoFixture.CriarDadosAguadouro();
            dadosCochoSal ??= PastoFixture.CriarDadosCochoSal();
        }

        return new(
            nome,
            area.Value,
            tipoPasto.Value,
            capacidadeMaxima.Value,
            dadosAguadouro,
            dadosCochoSal,
            temSombra.Value,
            observacoes
        );
    }
}
