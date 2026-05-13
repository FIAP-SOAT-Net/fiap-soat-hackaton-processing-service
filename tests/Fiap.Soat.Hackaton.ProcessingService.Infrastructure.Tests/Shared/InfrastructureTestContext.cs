using Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.Hackaton.ProcessingService.Infrastructure.Tests.Shared;

public static class InfrastructureTestContext
{
    public static SqliteContextScope CreateSqliteContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return new SqliteContextScope(context, connection);
    }

    public sealed class SqliteContextScope(AppDbContext context, SqliteConnection connection) : IDisposable, IAsyncDisposable
    {
        public AppDbContext Context { get; } = context;
        public SqliteConnection Connection { get; } = connection;

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Context.Dispose();
            return Connection.DisposeAsync();
        }
    }
}

