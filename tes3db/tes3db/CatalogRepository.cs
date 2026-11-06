namespace tes3db;

using System.Data.Common;
using static tes3db.CatalogRepository;

/// <summary>
/// Parameterless queries
/// (search index, expansion headers).
/// </summary>
public interface ICatalogRepository
{
    Task<IReadOnlyList<Dictionary<string, object?>>> QueryToDictionaryAsync(string query, List<QueryParameter> parameters, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetStringListAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDictionaryStringListAsync(string query, CancellationToken ct = default);
}

public sealed class CatalogRepository(IDbConnectionFactory db) : ICatalogRepository
{
    /// <summary>
    /// Executes a SQL query and returns all rows of the first column as a read-only string list.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="ct">A token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result is a read-only list 
    /// containing all values from the first column of the result set, in row order.
    /// </returns>
    public Task<IReadOnlyList<string>> GetStringListAsync(string query, CancellationToken ct = default)
    {
        return QueryAsync(
            query,
            r => r.GetString(0),
            ct
        );
    }


    /// <summary>
    /// Executes a SQL query and returns the results as a dictionary grouped by the first column.
    /// </summary>
    /// <param name="query">The SQL query to execute.</param>
    /// <param name="ct">A token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result is a dictionary 
    /// where each key corresponds to the first column of the result set, and the 
    /// value is a list of all corresponding entries from the second column.
    /// </returns>
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetDictionaryStringListAsync(string query, CancellationToken ct = default)
    {
        return QueryAsync(
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
    }

    /// <summary>
    /// Executes a SQL query and maps each row to an instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to map each row to.</typeparam>
    /// <param name="sql">The SQL command text.</param>
    /// <param name="map">A delegate that maps a <see cref="DbDataReader"/> row to an instance of <typeparamref name="T"/>.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The result is a read-only list 
    /// containing all mapped rows from the result set.
    /// </returns>
    private async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, Func<DbDataReader, T> map, CancellationToken ct)
    {
        await using var connection = await db.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync(ct);

        var results = new List<T>();

        while (await reader.ReadAsync(ct)) results.Add(map(reader));

        return results;
    }

    ///// <summary>
    ///// Executes a parameterized SQL query and maps each row to a.
    ///// </summary>
    ///// <typeparam name="T">The type to map each row to.</typeparam>
    ///// <param name="sql">The SQL command text, which may include named parameter placeholders (e.g. <c>@id</c>).</param>
    ///// <param name="parameters">A list of query parameters to bind to the command.</param>
    ///// <param name="ct">The cancellation token.</param>
    ///// <returns>
    ///// A task representing the asynchronous operation. The result is a read-only list 
    ///// containing all rows mapped to a dictionary.
    ///// </returns>
    public async Task<IReadOnlyList<Dictionary<string, object?>>> QueryToDictionaryAsync(string sql, List<QueryParameter> parameters, CancellationToken ct)
    {
        await using var connection = await db.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var param in parameters)
        {
            var p = command.CreateParameter();
            p.ParameterName = param.Name;
            p.Value = param.Value ?? DBNull.Value;
            command.Parameters.Add(p);
        }
        await using var reader = await command.ExecuteReaderAsync(ct);
        var results = new List<Dictionary<string, object?>>();
        while (await reader.ReadAsync(ct)) results.Add(RowToDict(reader));
        
        return results;
    }

    /// <summary>
    /// Converts a <see cref="DbDataReader"/> row to a Dictionary.
    /// </summary>
    /// <param name="r">A <see cref="DbDataReader"/> instance.</param>
    /// <returns>
    /// A Dictionary representing a single row from the result set.
    /// </returns>
    public static Dictionary<string, object?> RowToDict(DbDataReader r)
    {
        var row = new Dictionary<string, object?>(r.FieldCount);
        for (int i = 0; i < r.FieldCount; i++)
            row[r.GetName(i)] = r.IsDBNull(i) ? null : r.GetValue(i);
        return row;
    }

    /// <summary>
    /// Represents a parameter to be passed as part of a SQL query.
    /// name should be in format "@parameterName" and value is the value to be bound to that parameter.
    /// </summary>
    public class QueryParameter
    {
        public string? Name { get; set; }
        public object? Value { get; set; }
    }
}