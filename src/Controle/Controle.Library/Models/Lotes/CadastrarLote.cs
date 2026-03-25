using Dunet;

namespace Controle.Library.Models.Lotes;

public record CadastrarLoteRequisicao(
    int PastoId,
    decimal Area,
    int CapacidadeMaxima,
    string? Observacoes
);

[Union]
public partial record CadastrarLoteResposta
{
    partial record Sucesso(int LoteId);

    partial record Erro(CadastrarLoteTipoErro TipoErro);
}

public enum CadastrarLoteTipoErro
{
    ErroInterno,
    PastoNaoEncontrado,
}
