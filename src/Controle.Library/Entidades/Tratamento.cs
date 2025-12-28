using Controle.Library.Enums;

namespace Controle.Library.Entidades;

public class Tratamento
{
    public int Id { get; init; }
    public int GadoId { get; init; }
    public TipoTratamento Tipo { get; init; }
    public string NomeProduto { get; init; }
    public string? Fabricante { get; init; }
    public string? Lote { get; init; }
    public DateTimeOffset DataAplicacao { get; init; }
    public DateTimeOffset? DataProximaAplicacao { get; init; }
    public string Dose { get; init; }
    public TipoAplicacaoTratamento TipoAplicacao { get; init; }
    public string ResponsavelAplicacao { get; init; }
    public string Motivo { get; init; }
    public string? Observacoes { get; init; }

    public DateTimeOffset DataInclusao { get; private set; }
    public DateTimeOffset? DataUltimaAtualizacao { get; init; }

    // Navegação
    public virtual Gado? Gado { get; init; }

    protected Tratamento()
    {
        NomeProduto = null!;
        Dose = null!;
        ResponsavelAplicacao = null!;
        Motivo = null!;
    }

    public Tratamento(
        int gadoId,
        TipoTratamento tipo,
        string nomeProduto,
        DateTimeOffset dataAplicacao,
        string dose,
        TipoAplicacaoTratamento tipoAplicacao,
        string responsavelAplicacao,
        string motivo
    )
    {
        GadoId = gadoId;
        Tipo = tipo;
        NomeProduto = nomeProduto;
        DataAplicacao = dataAplicacao;
        Dose = dose;
        TipoAplicacao = tipoAplicacao;
        ResponsavelAplicacao = responsavelAplicacao;
        Motivo = motivo;
        DataInclusao = DateTimeOffset.UtcNow;
    }
}
