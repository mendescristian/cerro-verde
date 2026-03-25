using static Controle.Library.Models.Pastos.AtualizarPastoComReformaRequisicao;

namespace Controle.Library.Fixture.Models.Pastos;

public static class AtualizarPastoComReformaFixture
{
    public static Programada CriarProgramada(
        int pastoId,
        DateTimeOffset? dataPrevisao = null,
        string? observacoes = null
    )
    {
        return new Programada(dataPrevisao ?? DateTimeOffset.UtcNow.AddMonths(1))
        {
            PastoId = pastoId,
            Observacoes = observacoes ?? "Reforma Programada Teste",
        };
    }

    public static Vigente CriarVigente(int pastoId, int loteDestinoId, string? observacoes = null)
    {
        return new Vigente(loteDestinoId)
        {
            PastoId = pastoId,
            Observacoes = observacoes ?? "Iniciando Reforma Teste",
        };
    }

    public static Finalizada CriarFinalizada(int pastoId, string? observacoes = null)
    {
        return new Finalizada()
        {
            PastoId = pastoId,
            Observacoes = observacoes ?? "Reforma Concluída Teste",
        };
    }
}
