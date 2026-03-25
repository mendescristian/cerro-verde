using Controle.Library.Contexto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Controle.Library.Models.Pastos.AtualizarInfraestruturaPastoResposta;
using Requisicao = Controle.Library.Models.Pastos.AtualizarInfraestruturaPastoRequisicao;
using Resposta = Controle.Library.Models.Pastos.AtualizarInfraestruturaPastoResposta;
using TipoErro = Controle.Library.Models.Pastos.AtualizarInfraestruturaPastoTipoErro;

namespace Controle.Library.Comandos.Pastos;

internal record AtualizarInfraestruturaPastoComando(Requisicao Requisicao) : IRequest<Resposta>;

internal sealed class AtualizarInfraestruturaPastoHandler(
    ControleContexto contexto,
    ILogger<AtualizarInfraestruturaPastoHandler> logger
) : IRequestHandler<AtualizarInfraestruturaPastoComando, Resposta>
{
    public async Task<Resposta> Handle(
        AtualizarInfraestruturaPastoComando comando,
        CancellationToken cancellationToken
    )
    {
        var requisicao = comando.Requisicao;

        try
        {
            var pastoExiste = await contexto.Pastos.AnyAsync(p => p.Id == requisicao.PastoId);
            if (!pastoExiste)
                return TipoErro.PastoNaoEncontrado;

            await contexto
                .Pastos.Where(p => p.Id == requisicao.PastoId)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(p => p.Area, requisicao.Area)
                        .SetProperty(p => p.CapacidadeMaxima, requisicao.CapacidadeMaxima)
                        .SetProperty(p => p.TemSombra, requisicao.TemSombra)
                        .SetProperty(p => p.TemAguadouro, requisicao.DadosAguadouro != null)
                        .SetProperty(p => p.DadosAguadouro, requisicao.DadosAguadouro)
                        .SetProperty(p => p.TemCochoSal, requisicao.DadosCochoSal != null)
                        .SetProperty(p => p.DadosCochoSal, requisicao.DadosCochoSal)
                        .SetProperty(p => p.Observacoes, requisicao.Observacoes)
                        .SetProperty(p => p.DataUltimaAtualizacao, DateTimeOffset.UtcNow)
                );

            return new Sucesso(requisicao.PastoId);
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "Erro ao atualizar infraestrutura do pasto {PastoId}",
                requisicao.PastoId
            );

            return TipoErro.ErroInterno;
        }
    }
}
