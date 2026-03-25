using Dunet;

namespace Controle.Library.Models.Lotes;

[Union]
public partial record ExcluirLoteResposta
{
    partial record Sucesso;

    partial record Erro(ExcluirLoteTipoErro TipoErro);
}

public enum ExcluirLoteTipoErro
{
    ErroInterno,
    LoteNaoEncontrado,
    LotePossuiGadoVinculado,
}
