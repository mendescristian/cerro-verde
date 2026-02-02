using Controle.Library.Comandos.Pastos;
using Controle.Library.Models.Pastos;
using MediatR;

namespace Controle.Library.Servicos;

public interface IPastoServico
{
    Task<CadastrarPastoResposta> CadastrarPastoAsync(CadastrarPastoRequisicao requisicao);
}

public class PastoServico(IMediator mediator) : IPastoServico
{
    public async Task<CadastrarPastoResposta> CadastrarPastoAsync(
        CadastrarPastoRequisicao requisicao
    ) => await mediator.Send(new CadastrarPastoComando(requisicao));
}
