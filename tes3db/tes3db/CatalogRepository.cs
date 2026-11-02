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
    Task<IReadOnlyList<string>> GetStringListAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDictionaryStringListAsync(string query, CancellationToken ct = default);
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
            ct
        );

    public Task<IReadOnlyList<HeaderObject>> GetHeadersAsync(CancellationToken ct = default) =>
        QueryAsync(
            SqlQueries.header_query,
            r => new HeaderObject(
                expansion: r.GetString(0),
                author: r.GetString(1),
                description: r.GetString(2)),
            ct
        );

    public Task<IReadOnlyList<string>> GetStringListAsync(string query,CancellationToken ct = default) =>
        QueryAsync(
            query,
            r => r.GetString(0),
            ct
        );

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDictionaryStringListAsync(string query, CancellationToken ct = default) =>
        QueryAsync(
            query,
            r => new { Key = r.GetString(0), Value = r.GetString(1) },
            ct
        ).ContinueWith(t =>
        {
            var dict = new Dictionary<string, List<string>>();
            foreach (var item in t.Result)
            {
                if (!dict.TryGetValue(item.Key, out var list))
                {
                    list = new List<string>();
                    dict[item.Key] = list;
                }
                list.Add(item.Value);
            }
            return (IReadOnlyDictionary<string, IReadOnlyList<string>>)dict.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<string>)kvp.Value);
        }, ct);

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