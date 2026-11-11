namespace tes3db;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public interface ICatalogService {
    
    Task<IReadOnlyList<Dictionary<string, object?>>> RunQueryAsync(string query, CancellationToken ct = default);
    Task<InterfaceObject> GetInterfaceObject(CancellationToken ct = default);
    Task<object> GetBirthsign(CancellationToken ct, string id);
    Task<object> GetItem(CancellationToken ct, string query, string id);
    Task<object> GetCell(CancellationToken ct, string id);
    Task<object> GetClass(CancellationToken ct, string id);
    Task<object> GetContainer(CancellationToken ct, string id);
    Task<object> GetCreature(CancellationToken ct, string id);

    Task<object> GetNpc(CancellationToken ct, string id);
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

        interfaceObject.Filters.Race = await repo.GetStringListAsync(SqlQueries.race_list_query, ct);
        interfaceObject.Filters.Class = await repo.GetStringListAsync(SqlQueries.class_list_query, ct);
        interfaceObject.Filters.Faction = await repo.GetStringListAsync(SqlQueries.faction_list_query, ct);
        interfaceObject.Filters.Expansion = await repo.GetStringListAsync(SqlQueries.expansion_list_query, ct);
        interfaceObject.Filters.Location = await repo.GetDictionaryStringListAsync(SqlQueries.location_list_query, ct);
        interfaceObject.Filters.MagicEffect = await repo.GetStringListAsync(SqlQueries.magic_effect_list_query, ct);
        return interfaceObject;
    }

    public async Task<object> GetItem(CancellationToken ct, string query, string id) {

        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };

        var data = await repo.QueryToDictionaryAsync(query, parameters, ct);

        var npcs = await repo.QueryToDictionaryAsync(SqlQueries.npc_inventory_query, parameters, ct);

        var creatures = await repo.QueryToDictionaryAsync(SqlQueries.creature_inventory_query, parameters, ct);

        var containers = await repo.QueryToDictionaryAsync(SqlQueries.container_inventory_query, parameters, ct);

        var cells = await repo.QueryToDictionaryAsync(SqlQueries.cell_inventory_query, parameters, ct);

        var lists = await repo.QueryToDictionaryAsync(SqlQueries.item_list_query, parameters, ct);

        var returnObject = new
        {
             data,
             npcs,
             creatures,
             containers,
             cells,
             lists
        };
        return returnObject;
    }

    public async Task<object> GetBirthsign(CancellationToken ct, string id)
    {
        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var data = await repo.QueryToDictionaryAsync(SqlQueries.birthsign_query, parameters, ct);

        var spells = await repo.QueryToDictionaryAsync(SqlQueries.birthsign_spell_query, parameters, ct);

        var returnObject = new
        {
            data,
            spells
        };
        return returnObject;
    }

    public async Task<object> GetCell(CancellationToken ct, string id)
    {
        var parameter1 = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var parameter2 = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id1", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id2", Value=id }
        };
        var parameter3 = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id1", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id2", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id3", Value=id }
        };
        var parameter4 = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id1", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id2", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id3", Value=id },
            new CatalogRepository.QueryParameter{ Name="@id4", Value=id }
        };

        var data = await repo.QueryToDictionaryAsync(SqlQueries.cell_query , parameter1, ct);

        var npcs = await repo.QueryToDictionaryAsync(SqlQueries.cell_npc_query, parameter1, ct);

        var containers = await repo.QueryToDictionaryAsync(SqlQueries.cell_container_query, parameter1, ct);

        var equipment = await repo.QueryToDictionaryAsync(SqlQueries.cell_equipment_query, parameter3, ct);

        var ingredients = await repo.QueryToDictionaryAsync(SqlQueries.cell_ingredient_query, parameter2, ct);

        var tools = await repo.QueryToDictionaryAsync(SqlQueries.cell_tool_query, parameter4, ct);

        var miscitems = await repo.QueryToDictionaryAsync(SqlQueries.cell_miscitem_query, parameter2, ct);

        var creatures = await repo.QueryToDictionaryAsync(SqlQueries.cell_creature_query, parameter1, ct);

        var lists = await repo.QueryToDictionaryAsync(SqlQueries.cell_leveledcreature_query, parameter1, ct);

        var returnObject = new
        {
            data,
            npcs,
            containers,
            equipment,
            ingredients,
            tools,
            miscitems,
            creatures,
            lists
        };
        return returnObject;
    }

    public async Task<object> GetClass(CancellationToken ct, string id)
    {
        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var data = await repo.QueryToDictionaryAsync(SqlQueries.class_query, parameters, ct);
        
        var npcs = await repo.QueryToDictionaryAsync(SqlQueries.npc_class_query, parameters, ct);

        var returnObject = new
        {
            data,
            npcs
        };
        return returnObject;
    }

    public async Task<object> GetContainer(CancellationToken ct, string id)
    {
        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var data = await repo.QueryToDictionaryAsync(SqlQueries.container_query, parameters, ct);

        var cells = await repo.QueryToDictionaryAsync(SqlQueries.container_cell_query, parameters, ct);
        
        var returnObject = new
        {
            data,
            cells
        };
        return returnObject;
    }

    public async Task<object> GetCreature(CancellationToken ct, string id)
    {
        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var data = await repo.QueryToDictionaryAsync(SqlQueries.creature_query, parameters, ct);

        var cells = await repo.QueryToDictionaryAsync(SqlQueries.cell_inventory_query, parameters, ct);

        var lists = await repo.QueryToDictionaryAsync(SqlQueries.creature_list_query, parameters, ct);

        var returnObject = new
        {
            data,
            cells,
            lists
        };
        return returnObject;
    }

    public async Task<object> GetNpc(CancellationToken ct, string id)
    {
        var parameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@id", Value=id }
        };
        var data = await repo.QueryToDictionaryAsync(SqlQueries.npc_query, parameters, ct);

        var dialogueParameters = new List<CatalogRepository.QueryParameter>
        {
            new CatalogRepository.QueryParameter{ Name="@speakerId", Value=id },
            new CatalogRepository.QueryParameter{ Name="@speakerId2", Value=id },
            new CatalogRepository.QueryParameter{ Name="@speakerId3", Value=id }
        };
        var dialogue = await repo.QueryToDictionaryAsync(SqlQueries.npc_dialogue_query, dialogueParameters, ct);


        var returnObject = new
        {
            data,
            dialogue
        };
        return returnObject;
    }
}

