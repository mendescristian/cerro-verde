using Controle.Library.Contexto;
using Controle.Library.Entidades;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Requisicao = Controle.Library.Models.Lotes.CadastrarLoteRequisicao;
using Resposta = Controle.Library.Models.Lotes.CadastrarLoteResposta;
using TipoErro = Controle.Library.Models.Lotes.CadastrarLoteTipoErro;

namespace Controle.Library.Comandos.Lotes;

internal record CadastrarLoteComando(Requisicao Requisicao) : IRequest<Resposta>;

internal sealed class CadastrarLoteHandler(
    ControleContexto contexto,
    ILogger<CadastrarLoteHandler> logger
) : IRequestHandler<CadastrarLoteComando, Resposta>
{
    public async Task<Resposta> Handle(
        CadastrarLoteComando comando,
        CancellationToken cancellationToken
    )
    {
        var requisicao = comando.Requisicao;

        try
        {
            var pastoExiste = await contexto.Pastos.AnyAsync(
                p => p.Id == requisicao.PastoId,
                cancellationToken
            );

            if (!pastoExiste)
                return TipoErro.PastoNaoEncontrado;

            var ultimoNumero = await contexto
                .Lotes.Where(l => l.PastoId == requisicao.PastoId)
                .MaxAsync(l => (int?)l.Numero);

            var novoNumero = (ultimoNumero ?? 0) + 1;

            var lote = new Lote(
                requisicao.PastoId,
                novoNumero,
                requisicao.Area,
                requisicao.CapacidadeMaxima,
                requisicao.Observacoes
            );

            contexto.Lotes.Add(lote);

            await contexto.SaveChangesAsync();

            return lote.Id;
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "Erro ao cadastrar lote para o pasto {PastoId}",
                requisicao.PastoId
            );

            return TipoErro.ErroInterno;
        }
    }
}
