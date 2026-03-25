using Dunet;

namespace Controle.Library.Models.Pastos;

[Union]
public partial record ExcluirPastoResposta
{
    partial record Sucesso;

    partial record Erro(ExcluirPastoTipoErro TipoErro);
}

public enum ExcluirPastoTipoErro
{
    ErroInterno,
    PastoNaoEncontrado,
    PastoPossuiGadoVinculado,
}
