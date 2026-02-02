using Controle.Library.Contexto;
using Controle.Library.Fixture.Entidades;

namespace Controle.Library.Fixture.Contexto;

public class ControleContextoFixture
{
    // csharpier-ignore-start
    public PastoFixture Pasto { get; init; }
    public LoteFixture Lote { get; init; }
    public GadoFixture Gado { get; init; }
    public MovimentacaoFixture Movimentacao { get; init; }
    // csharpier-ignore-end

    internal ControleContextoFixture(ControleContexto contexto)
    {
        Pasto = new PastoFixture(contexto);
        Lote = new LoteFixture(contexto, Pasto);
        Gado = new GadoFixture(contexto, Lote);
        Movimentacao = new MovimentacaoFixture(contexto, Gado, Lote);
    }
}
