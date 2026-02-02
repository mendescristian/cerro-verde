using Controle.Library.Contexto;
using Controle.Library.Servicos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Controle.Library;

public static class IoC
{
    public static IServiceCollection InjetarDependenciasControle(
        this IServiceCollection servicos,
        IConfiguration configuracao
    )
    {
        servicos.TryAddSingleton(TimeProvider.System);

        servicos.TryAddScoped<IPastoServico, PastoServico>();

        servicos.AddDbContext<ControleContexto>(opts =>
            opts.ConfigurarContexto(configuracao["DATABASE_CONTROLE_POSTGRES"]!)
        );

        return servicos;
    }
}
