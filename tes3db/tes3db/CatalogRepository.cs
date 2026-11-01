namespace tes3db;

using System.Data.Common;

/// <summary>
/// Parameterless queries
/// (search index, expansion headers).
/// </summary>
public interface ICatalogRepository
{
    Task<IReadOnlyList<SearchObject>> GetSearchObjectsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<HeaderObject>> GetHeadersAsync(CancellationToken ct = default);
}

public sealed class CatalogRepository(IDbConnectionFactory db) : ICatalogRepository
{
    public Task<IReadOnlyList<SearchObject>> GetSearchObjectsAsync(CancellationToken ct = default) =>
        QueryAsync(
            SqlQueries.search_object_query,
            r => new SearchObject(
                id: r.GetString(0),
                name: r.GetString(1),
                type: r.GetString(2)),
            ct);

    public Task<IReadOnlyList<HeaderObject>> GetHeadersAsync(CancellationToken ct = default) =>
        QueryAsync(
            SqlQueries.header_query,
            r => new HeaderObject(
                expansion: r.GetString(0),
                author: r.GetString(1),
                description: r.GetString(2)),
            ct);

    // Shared open / execute / map loop so each query is just SQL + a row mapper
    private async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<DbDataReader, T> map, CancellationToken ct)
    {
        await using var connection = await db.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        await using var reader = await command.ExecuteReaderAsync(ct);

        var results = new List<T>();
        while (await reader.ReadAsync(ct))
            results.Add(map(reader));
        return results;
    }
}