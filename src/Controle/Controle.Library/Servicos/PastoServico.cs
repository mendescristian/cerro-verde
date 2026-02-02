using Controle.Library.Comandos.Pastos;
using Controle.Library.Models.Pastos;
using MediatR;

namespace Controle.Library.Servicos;

public interface IPastoServico
{
    Task<CadastrarPastoResposta> CadastrarPastoAsync(CadastrarPastoRequisicao requisicao);

    Task<AtualizarInfraestruturaPastoResposta> AtualizarInfraestruturaPastoAsync(
        AtualizarInfraestruturaPastoRequisicao requisicao
    );

    Task<AtualizarPastoComReformaResposta> AtualizarPastoComReformaAsync(
        AtualizarPastoComReformaRequisicao requisicao
    );
}

public class PastoServico(IMediator mediator) : IPastoServico
{
    public async Task<CadastrarPastoResposta> CadastrarPastoAsync(
        CadastrarPastoRequisicao requisicao
    ) => await mediator.Send(new CadastrarPastoComando(requisicao));

    public async Task<AtualizarInfraestruturaPastoResposta> AtualizarInfraestruturaPastoAsync(
        AtualizarInfraestruturaPastoRequisicao requisicao
    ) => await mediator.Send(new AtualizarInfraestruturaPastoComando(requisicao));

    public async Task<AtualizarPastoComReformaResposta> AtualizarPastoComReformaAsync(
        AtualizarPastoComReformaRequisicao requisicao
    ) => await mediator.Send(new AtualizarPastoComReformaComando(requisicao));
}
