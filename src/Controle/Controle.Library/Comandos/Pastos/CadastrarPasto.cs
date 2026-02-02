using Controle.Library.Contexto;
using Controle.Library.Entidades;
using Controle.Library.Models.Pastos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Controle.Library.Models.Pastos.CadastrarPastoResposta;
using Requisicao = Controle.Library.Models.Pastos.CadastrarPastoRequisicao;
using Resposta = Controle.Library.Models.Pastos.CadastrarPastoResposta;

namespace Controle.Library.Comandos.Pastos;

internal record CadastrarPastoComando(Requisicao Requisicao) : IRequest<Resposta>;

internal sealed class CadastrarPastoHandler(
    ControleContexto contexto,
    ILogger<CadastrarPastoHandler> logger
) : IRequestHandler<CadastrarPastoComando, Resposta>
{
    public async Task<Resposta> Handle(
        CadastrarPastoComando comando,
        CancellationToken cancellationToken
    )
    {
        var requisicao = comando.Requisicao;

        try
        {
            if (await contexto.Pastos.AnyAsync(p => p.Nome == requisicao.Nome))
                return new Erro(CadastrarPastoTipoErro.NomeJaCadastrado);

            var pasto = new Pasto(
                requisicao.Nome,
                requisicao.Area,
                requisicao.TipoPasto,
                requisicao.CapacidadeMaxima,
                requisicao.DadosAguadouro,
                requisicao.DadosCochoSal,
                requisicao.TemSombra,
                requisicao.Observacoes
            );

            contexto.Pastos.Add(pasto);
            await contexto.SaveChangesAsync();

            return new Sucesso(pasto.Id);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Erro ao cadastrar pasto {NomePasto}", requisicao.Nome);

            return new Erro(CadastrarPastoTipoErro.ErroInterno);
        }
    }
}
