using Controle.Library.Entidades;
using Dunet;

namespace Controle.Library.Models.Pastos;

public record AtualizarInfraestruturaPastoRequisicao(
    int PastoId,
    decimal Area,
    int CapacidadeMaxima,
    DadosAguadouroPasto? DadosAguadouro,
    DadosCochoSalPasto? DadosCochoSal,
    bool TemSombra,
    string? Observacoes
);

[Union]
public partial record AtualizarInfraestruturaPastoResposta
{
    partial record Sucesso(int PastoId);

    partial record Erro(AtualizarInfraestruturaPastoTipoErro TipoErro);
}

public enum AtualizarInfraestruturaPastoTipoErro
{
    ErroInterno,
    PastoNaoEncontrado,
}
