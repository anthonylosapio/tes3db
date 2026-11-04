namespace tes3db;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public interface ICatalogService { 
    Task<InterfaceObject> GetInterfaceObject(CancellationToken ct = default);
}
public class CatalogService(ICatalogRepository repo) : ICatalogService
{

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
}

