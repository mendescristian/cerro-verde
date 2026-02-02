using Controle.Library.Contexto;
using Controle.Library.Fixture.Contexto;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TUnit.Core.Interfaces;

namespace Controle.Library.Fixture.Providers;

public class ControleContextoProvider : IAsyncInitializer, IAsyncDisposable, ITestEndEventReceiver
{
    private readonly string _nomeBancoTeste;

    internal ControleContexto Contexto { get; private set; } = null!;

    private bool _disposed = false;

    [ClassDataSource<PostgresContainerProvider>(Shared = SharedType.PerTestSession)]
    public required PostgresContainerProvider PostgresContainerProvider { get; init; }

    public string ConnectionString { get; private set; } = null!;
    public ControleContextoFixture Fixture { get; private set; } = null!;

    public ControleContextoProvider()
    {
        _nomeBancoTeste = $"test_{Guid.NewGuid():N}";
    }

    public async Task InitializeAsync()
    {
        await CriarBancoTesteAsync();

        ConnectionString = PostgresContainerProvider.ObterConnectionString(_nomeBancoTeste);

        Contexto = CriarContexto();

        Fixture = new ControleContextoFixture(Contexto);
    }

    internal ControleContexto CriarContexto()
    {
        var options = new DbContextOptionsBuilder<ControleContexto>()
            .ConfigurarContexto(ConnectionString)
            .Options;

        return new ControleContexto((DbContextOptions<ControleContexto>)options);
    }

    private async Task CriarBancoTesteAsync()
    {
        await using var conexaoAdmin = await PostgresContainerProvider.CriarConexaoAdminAsync();

        var createDbCommand =
            $"CREATE DATABASE \"{_nomeBancoTeste}\" WITH TEMPLATE \"{PostgresContainerProvider.DatabaseName}\"";

        using var command = new NpgsqlCommand(createDbCommand, conexaoAdmin);

        await command.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await Contexto.DisposeAsync();

        await using var conexao = await PostgresContainerProvider.CriarConexaoAdminAsync();

        await MatarConexoesBancoAsync(conexao);

        await DeletarBancoAsync(conexao);

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private async Task MatarConexoesBancoAsync(NpgsqlConnection conexao)
    {
        var query =
            $@"
            SELECT pg_terminate_backend(pg_stat_activity.pid)
            FROM pg_stat_activity
            WHERE pg_stat_activity.datname = '{_nomeBancoTeste}'
              AND pid <> pg_backend_pid()";

        await using var comando = new NpgsqlCommand(query, conexao);

        await comando.ExecuteNonQueryAsync();
    }

    private async Task DeletarBancoAsync(NpgsqlConnection conexao)
    {
        var query = $"DROP DATABASE IF EXISTS \"{_nomeBancoTeste}\"";

        var comando = new NpgsqlCommand(query, conexao);

        await comando.ExecuteNonQueryAsync();
    }

    public async ValueTask OnTestEnd(TestContext context)
    {
        await DisposeAsync();
    }
}
