using Controle.Library.Comandos.Lotes;
using Controle.Library.Models.Lotes;
using MediatR;

namespace Controle.Library.Servicos;

public interface ILoteServico
{
    Task<CadastrarLoteResposta> CadastrarLoteAsync(CadastrarLoteRequisicao requisicao);
    Task<AtualizarLoteResposta> AtualizarLoteAsync(AtualizarLoteRequisicao requisicao);
}

public class LoteServico(IMediator mediator) : ILoteServico
{
    public async Task<CadastrarLoteResposta> CadastrarLoteAsync(
        CadastrarLoteRequisicao requisicao
    ) => await mediator.Send(new CadastrarLoteComando(requisicao));

    public async Task<AtualizarLoteResposta> AtualizarLoteAsync(
        AtualizarLoteRequisicao requisicao
    ) => await mediator.Send(new AtualizarLoteComando(requisicao));
}
