using Controle.Library.Entidades;
using Controle.Library.Enums;
using Dunet;

namespace Controle.Library.Models.Pastos;

public record CadastrarPastoRequisicao(
    string Nome,
    decimal Area,
    TipoPasto TipoPasto,
    int CapacidadeMaxima,
    DadosAguadouroPasto? DadosAguadouro,
    DadosCochoSalPasto? DadosCochoSal,
    bool TemSombra,
    string? Observacoes
);

[Union]
public partial record CadastrarPastoResposta
{
    partial record Sucesso(int PastoId);

    partial record Erro(CadastrarPastoTipoErro TipoErro);
}

public enum CadastrarPastoTipoErro
{
    ErroInterno,
    NomeJaCadastrado,
}
