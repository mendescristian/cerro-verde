using Controle.Library.Enums;
using Dunet;

namespace Controle.Library.Models.Lotes;

public record AtualizarLoteRequisicao(
    int LoteId,
    decimal Area,
    int CapacidadeMaxima,
    StatusLote Status,
    string? Observacoes
);

[Union]
public partial record AtualizarLoteResposta
{
    partial record Sucesso(int LoteId);

    partial record Erro(AtualizarLoteTipoErro TipoErro);
}

public enum AtualizarLoteTipoErro
{
    ErroInterno,
    LoteNaoEncontrado,
}
