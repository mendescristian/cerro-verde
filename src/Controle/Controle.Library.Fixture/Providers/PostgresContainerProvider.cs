using Controle.Library.Contexto;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace Controle.Library.Fixture.Providers;

public class PostgresContainerProvider : IAsyncInitializer, IAsyncDisposable
{
    public const string DatabaseName = "db_controle_template";
    private PostgreSqlContainer? _container;

    public PostgreSqlContainer Container
    {
        get
        {
            return _container ??= new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase(DatabaseName)
                .Build();
        }
        set { _container = value; }
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        await CriarBancoTemplateAsync();
    }

    private async Task CriarBancoTemplateAsync()
    {
        await AplicarMigrationsAsync();

        await using NpgsqlConnection conexao = await IniciarConexaoAdmin();

        await LimitarConexoesBancoTemplateAsync(conexao);

        await MatarConexoesBancoTemplateAsync(conexao);
    }

    private async Task AplicarMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<ControleContexto>()
            .ConfigurarContexto(Container.GetConnectionString())
            .Options;

        await using var contexto = new ControleContexto(
            (DbContextOptions<ControleContexto>)options
        );

        await contexto.Database.EnsureCreatedAsync();
    }

    private async Task<NpgsqlConnection> IniciarConexaoAdmin()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(Container.GetConnectionString())
        {
            Database = "postgres",
        }.ConnectionString;

        var conexao = new NpgsqlConnection(connectionString);

        await conexao.OpenAsync();

        return conexao;
    }

    private static async Task LimitarConexoesBancoTemplateAsync(NpgsqlConnection conexao)
    {
        var query =
            $@"
            ALTER DATABASE ""{DatabaseName}"" WITH
            CONNECTION LIMIT 0
            IS_TEMPLATE true";

        await using var comando = new NpgsqlCommand(query, conexao);

        await comando.ExecuteNonQueryAsync();
    }

    private static async Task MatarConexoesBancoTemplateAsync(NpgsqlConnection conexao)
    {
        var query =
            $@"
            SELECT pg_terminate_backend(pg_stat_activity.pid)
            FROM pg_stat_activity
            WHERE pg_stat_activity.datname = '{DatabaseName}'
              AND pid <> pg_backend_pid()";

        await using var comando = new NpgsqlCommand(query, conexao);

        await comando.ExecuteNonQueryAsync();
    }

    public string ObterConnectionString(string nomeBanco)
    {
        var builder = new NpgsqlConnectionStringBuilder(Container.GetConnectionString())
        {
            Database = nomeBanco,
        };

        return builder.ConnectionString;
    }

    public async Task<NpgsqlConnection> CriarConexaoAdminAsync()
    {
        var conexao = new NpgsqlConnection(ObterConnectionString("postgres"));

        await conexao.OpenAsync();

        return conexao;
    }

    public async ValueTask DisposeAsync()
    {
        if (_container != null)
            await _container.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
