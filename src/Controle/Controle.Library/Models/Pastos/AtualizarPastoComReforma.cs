using Dunet;

namespace Controle.Library.Models.Pastos;

[Union]
public partial record AtualizarPastoComReformaRequisicao
{
    public required int PastoId;
    public required string? Observacoes;

    partial record Programada(DateTimeOffset DataPrevisao);

    partial record Vigente(int LoteDestinoId);

    partial record Finalizada;
}

[Union]
public partial record AtualizarPastoComReformaResposta
{
    partial record Sucesso(int PastoId);

    partial record Erro(AtualizarPastoComReformaTipoErro TipoErro);
}

public enum AtualizarPastoComReformaTipoErro
{
    ErroInterno,
    PastoNaoEncontrado,
    LoteDestinoNaoEncontrado,
    PastoAindaPossuiGado,
    StatusInvalidoParaFinalizar,
}
