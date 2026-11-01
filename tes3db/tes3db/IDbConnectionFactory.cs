namespace tes3db;

using System.Data.Common;
using Microsoft.Data.Sqlite;

public interface IDbConnectionFactory
{
    Task<DbConnection> OpenAsync(CancellationToken ct = default);
}

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string dbFilePath)
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbFilePath,
            Mode = SqliteOpenMode.ReadOnly
        }.ToString();
    }

    public async Task<DbConnection> OpenAsync(CancellationToken ct = default)
    {
        var connection = new SqliteConnection(_connectionString);
        try
        {
            await connection.OpenAsync(ct);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }
}