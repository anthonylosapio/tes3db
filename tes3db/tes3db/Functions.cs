namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text.Json;
using static tes3db.Models;

public class Functions
{
    public static Attributes SetAttributes(JsonElement element)
    {
        Attributes attributes = new Attributes();
        if (element.ValueKind != JsonValueKind.Array)
        {
            return attributes;
        }
        attributes.Strength = element[0].GetInt32();
        attributes.Intelligence = element[1].GetInt32();
        attributes.Willpower = element[2].GetInt32();
        attributes.Agility = element[3].GetInt32();
        attributes.Speed = element[4].GetInt32();
        attributes.Endurance = element[5].GetInt32();
        attributes.Personality = element[6].GetInt32();
        attributes.Luck = element[7].GetInt32();
        return attributes;
    }

    public static List<InventoryItem> GetInventory(JsonElement element)
    {
        List<InventoryItem> inventory = new List<InventoryItem>();
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement array in element.EnumerateArray())
            {
                InventoryItem inventoryItem = new InventoryItem();
                inventoryItem.Quantity = array[0].GetInt32();
                inventoryItem.ItemId = array[1].GetString();
                inventory.Add(inventoryItem);
            }
        }
        return inventory;
    }

    public static Skills SetSkills(JsonElement element)
    {
        Skills skills = new Skills();
        if (element.ValueKind != JsonValueKind.Array)
        {
            return skills; // missing/null "attributes" -> defaults (all zero)
        }
        skills.Block = element[0].GetInt32();
        skills.Armorer = element[1].GetInt32();
        skills.MediumArmor = element[2].GetInt32();
        skills.HeavyArmor = element[3].GetInt32();
        skills.BluntWeapon = element[4].GetInt32();
        skills.LongBlade = element[5].GetInt32();
        skills.Axe = element[6].GetInt32();
        skills.Spear = element[7].GetInt32();
        skills.Athletics = element[8].GetInt32();
        skills.Enchant = element[9].GetInt32();
        skills.Destruction = element[10].GetInt32();
        skills.Alteration = element[11].GetInt32();
        skills.Illusion = element[12].GetInt32();
        skills.Conjuration = element[13].GetInt32();
        skills.Mysticism = element[14].GetInt32();
        skills.Restoration = element[15].GetInt32();
        skills.Alchemy = element[16].GetInt32();
        skills.Unarmored = element[17].GetInt32();
        skills.Security = element[18].GetInt32();
        skills.Sneak = element[19].GetInt32();
        skills.Acrobatics = element[20].GetInt32();
        skills.LightArmor = element[21].GetInt32();
        skills.ShortBlade = element[22].GetInt32();
        skills.Marksman = element[23].GetInt32();
        skills.Mercantile = element[24].GetInt32();
        skills.Speechcraft = element[25].GetInt32();
        skills.HandToHand = element[26].GetInt32();

        return skills;
    }

    public static List<string> GetCellRefs(JsonElement element)
    {
        List<string> refs = new List<string>();
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement obj in element.EnumerateArray())
            {
                if (obj.TryGetProperty("id", out JsonElement id))
                {
                    var idString = id.GetString();
                    if (!string.IsNullOrEmpty(idString)) refs.Add(idString);
                }
            }
        }
        return refs;
    }

    public static T DeserializeObject<T>(JsonElement element) where T : new()
    {
        try
        {
            return JsonSerializer.Deserialize<T>(element)
                ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}.");
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.ToString());
            return new T();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.ToString());
            return new T();
        }
    }

    public static Dictionary<string, Cell> BuildReferenceIndex(List<Cell> cells, string expansion)
    {
        var index = new Dictionary<string, Cell>();
        foreach (var cell in cells)
        {
            if (cell.expansion != expansion || cell.references == null)
                continue;

            foreach (var reference in cell.references)
            {
                // last cell wins on duplicate references; adjust if you need first-wins instead
                index[reference] = cell;
            }
        }
        return index;
    }

    public static void AddCellLocationInfoToNPC(Npc npc, Dictionary<string, Cell> referenceIndex)
    {
        if (referenceIndex.TryGetValue(npc.id, out var cell))
        {
            npc.region = cell.region;
            npc.cell = cell.name;
            npc.location = cell.location;
            npc.sub_location = cell.sub_location;
        }
    }

    public static void AddRaceInfoToNPC(Npc npc, List<Race> races)
    {
        foreach (var race in races)
        {
            if (race.id == npc.race_id)
            {
                npc.race = race.name;
                return;
            }
        }
    }

    public static void AddClassInfoToNPC(Npc npc, List<Class> classes)
    {
        foreach (var cls in classes)
        {
            if (cls.id == npc.class_id)
            {
                npc.classs = cls.name;
                return;
            }
        }
    }

    public static void AddFactionInfoToNPC(Npc npc, List<Faction> factions)
    {
        foreach (var faction in factions)
        {
            if (faction.id == npc.faction_id)
            {
                npc.faction = faction.name;
                return;
            }
        }
        npc.faction = "None";
    }
}