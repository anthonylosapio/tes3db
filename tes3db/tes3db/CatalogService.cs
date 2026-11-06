namespace tes3db;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public interface ICatalogService {
    
    Task<IReadOnlyList<Dictionary<string, object?>>> RunQueryAsync(string query, CancellationToken ct = default);

    Task<InterfaceObject> GetInterfaceObject(CancellationToken ct = default);

    Task<object> GetAlchemy(CancellationToken ct, string id);
}
public class CatalogService(ICatalogRepository repo) : ICatalogService
{

    public async Task<IReadOnlyList<Dictionary<string, object?>>> RunQueryAsync(string query, CancellationToken ct) 
    {
        var result = await repo.QueryToDictionaryAsync(query, new List<CatalogRepository.QueryParameter>(), ct);
        return result;
    }

    public async Task<InterfaceObject> GetInterfaceObject(CancellationToken ct)
    {
        var interfaceObject = new InterfaceObject();

        interfaceObject.Filters.Race = await repo.GetStringListAsync(SqlQueries.race_query, ct);
        interfaceObject.Filters.Class = await repo.GetStringListAsync(SqlQueries.class_query, ct);
        interfaceObject.Filters.Faction = await repo.GetStringListAsync(SqlQueries.faction_query, ct);
        interfaceObject.Filters.Expansion = await repo.GetStringListAsync(SqlQueries.expansion_query, ct);
        interfaceObject.Filters.Location = await repo.GetDictionaryStringListAsync(SqlQueries.location_query, ct);
        interfaceObject.Filters.MagicEffect = await repo.GetStringListAsync(SqlQueries.magic_effect_query, ct);
        return interfaceObject;
    }

    public async Task<object> GetAlchemy(CancellationToken ct, string id) {

        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };

        var data = await repo.QueryToDictionaryAsync("SELECT * FROM alchemy WHERE id = @id", parameters, ct);

        var npc = await repo.QueryToDictionaryAsync(SqlQueries.npc_inventory_query, parameters, ct);

        var returnObject = new
        {
             data,
             npc

        };
        return returnObject;
    }
}

