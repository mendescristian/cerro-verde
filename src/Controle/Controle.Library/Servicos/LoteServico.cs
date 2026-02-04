using Controle.Library.Comandos.Lotes;
using Controle.Library.Models.Lotes;
using MediatR;

namespace Controle.Library.Servicos;

public interface ILoteServico
{
    Task<CadastrarLoteResposta> CadastrarLoteAsync(CadastrarLoteRequisicao requisicao);
}

public class LoteServico(IMediator mediator) : ILoteServico
{
    public async Task<CadastrarLoteResposta> CadastrarLoteAsync(
        CadastrarLoteRequisicao requisicao
    ) => await mediator.Send(new CadastrarLoteComando(requisicao));
}
