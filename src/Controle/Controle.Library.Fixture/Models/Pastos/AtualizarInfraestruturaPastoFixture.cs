using Controle.Library.Entidades;
using Controle.Library.Fixture.Entidades;
using Controle.Library.Models.Pastos;
using Testes.Common.Utils;

namespace Controle.Library.Fixture.Models.Pastos;

public static class AtualizarInfraestruturaPastoFixture
{
    public static AtualizarInfraestruturaPastoRequisicao CriarRequisicao(
        int? pastoId = null,
        decimal? area = null,
        int? capacidadeMaxima = null,
        DadosAguadouroPasto? dadosAguadouro = null,
        DadosCochoSalPasto? dadosCochoSal = null,
        bool? temSombra = null,
        string? observacoes = null,
        bool gerarInfraestrutura = true
    )
    {
        pastoId ??= FakeNumbers.CriarInt();
        area ??= FakeNumbers.CriarDecimal();
        capacidadeMaxima ??= FakeNumbers.CriarInt();
        temSombra ??= true;
        observacoes ??= "Observação Atualizada";

        if (gerarInfraestrutura)
        {
            dadosAguadouro ??= PastoFixture.CriarDadosAguadouro(quantidadeAferida: 999);
            dadosCochoSal ??= PastoFixture.CriarDadosCochoSal(quantidadeAferida: 888);
        }

        return new AtualizarInfraestruturaPastoRequisicao(
            pastoId.Value,
            area.Value,
            capacidadeMaxima.Value,
            dadosAguadouro,
            dadosCochoSal,
            temSombra.Value,
            observacoes
        );
    }
}
