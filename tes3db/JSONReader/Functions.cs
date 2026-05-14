using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static MorrowindNPCExtractor.dataextractor.Models;

namespace MorrowindNPCExtractor.dataextractor
{
    public class Functions
    {
        public static Models.Expansion SetExpansion(string expansionName, JsonElement element)
        {
            Models.Expansion expansion = new Models.Expansion();
            expansion.Name = expansionName;
            if (element.TryGetProperty("id", out JsonElement id)) expansion.NPCId = id.GetString();
            return expansion;
        }

        public static Models.Npc newNpc(JsonElement element)
        {
            Models.Npc newNpc = new Models.Npc();
            try
            {
                if (element.TryGetProperty("id", out JsonElement id)) newNpc.Id = id.GetString();
                if (element.TryGetProperty("name", out JsonElement name)) newNpc.Name = name.GetString();
                //inventory
                if (element.TryGetProperty("inventory", out JsonElement inventory)) newNpc.Inventory = GetInventory(inventory);
                //spells
                if (element.TryGetProperty("spells", out JsonElement spells)) newNpc.Spells = GetSpells(spells);
                //travel destinations
                if (element.TryGetProperty("travel_destinations", out JsonElement travel)) newNpc.OFFERS_TRAVEL = (travel.ValueKind == JsonValueKind.Array && travel.GetArrayLength() == 0) ? false : true;
                //ai data
                if (element.TryGetProperty("ai_data", out JsonElement ai_data))
                {
                    if (ai_data.TryGetProperty("hello", out JsonElement hello)) newNpc.Hello = hello.GetInt32();
                    if (ai_data.TryGetProperty("fight", out JsonElement fight)) newNpc.Fight = fight.GetInt32();
                    if (ai_data.TryGetProperty("flee", out JsonElement flee)) newNpc.Flee = flee.GetInt32();
                    if (ai_data.TryGetProperty("alarm", out JsonElement alarm)) newNpc.Alarm = alarm.GetInt32();
                    if (ai_data.TryGetProperty("services", out JsonElement services)) newNpc = AddServices(newNpc, services.GetString() ?? string.Empty);
                }
                if (element.TryGetProperty("race", out JsonElement race)) newNpc.Race = race.GetString();
                if (element.TryGetProperty("class", out JsonElement cl)) newNpc.Class = cl.GetString();
                if (element.TryGetProperty("faction", out JsonElement faction)) newNpc.Faction = faction.GetString();
                if (element.TryGetProperty("npc_flags", out JsonElement npc_flags)) newNpc.NpcFlags = npc_flags.GetString();
                newNpc.Gender = ((newNpc.NpcFlags ?? string.Empty).Contains("FEMALE")) ? "FEMALE" : "MALE";
                newNpc.IsEssential = ((newNpc.NpcFlags ?? string.Empty).Contains("ESSENTIAL ")) ? true : false;
                if (element.TryGetProperty("flags", out JsonElement flags)) newNpc.IsPersistent = ((flags.GetString() ?? string.Empty).Contains("PERSISTENT")) ? true : false;
                //data
                if (element.TryGetProperty("data", out JsonElement data))
                {
                    if (data.TryGetProperty("level", out JsonElement level)) newNpc.Level = level.GetInt32();
                    //stats
                    if (data.TryGetProperty("stats", out JsonElement stats))
                    {
                        if (stats.TryGetProperty("attributes", out JsonElement attributes)) newNpc.Attributes = SetAttributes(attributes);
                        //skills
                        if (stats.TryGetProperty("skills", out JsonElement skills)) newNpc.Skills = SetSkills(skills);
                        if (stats.TryGetProperty("health", out JsonElement health)) newNpc.Health = health.GetInt32();
                        if (stats.TryGetProperty("magicka", out JsonElement magicka)) newNpc.Magicka = magicka.GetInt32();
                        if (stats.TryGetProperty("fatigue", out JsonElement fatigue)) newNpc.Fatigue = fatigue.GetInt32();
                    }
                    if (data.TryGetProperty("disposition", out JsonElement disposition)) newNpc.Disposition = disposition.GetInt32();
                    if (data.TryGetProperty("reputation", out JsonElement reputation)) newNpc.Reputation = reputation.GetInt32();
                    if (data.TryGetProperty("rank", out JsonElement rank)) newNpc.Rank = rank.GetInt32();
                    if (data.TryGetProperty("gold", out JsonElement gold)) newNpc.Gold = gold.GetInt32();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating NPC: {ex.Message}");
            }
            return newNpc;
        }

        public static Models.Attributes SetAttributes(JsonElement element)
        {
            Attributes attributes = new Attributes();
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

        public static Models.Npc AddServices(Models.Npc npc, string services)
        {
            npc.BARTERS_WEAPONS = (services.Contains("BARTERS_WEAPONS")) ? true : false;
            npc.BARTERS_ARMOR = (services.Contains("BARTERS_ARMOR")) ? true : false;
            npc.BARTERS_REPAIR_ITEMS = (services.Contains("BARTERS_REPAIR_ITEMS")) ? true : false;
            npc.BARTERS_INGREDIENTS = (services.Contains("BARTERS_INGREDIENTS")) ? true : false;
            npc.BARTERS_ALCHEMY = (services.Contains("BARTERS_ALCHEMY")) ? true : false;
            npc.BARTERS_BOOKS = (services.Contains("BARTERS_BOOKS")) ? true : false;
            npc.BARTERS_CLOTHING = (services.Contains("BARTERS_CLOTHING")) ? true : false;
            npc.BARTERS_LIGHTS = (services.Contains("BARTERS_LIGHTS")) ? true : false;
            npc.BARTERS_MISC_ITEMS = (services.Contains("BARTERS_MISC_ITEMS")) ? true : false;
            npc.BARTERS_LOCKPICKS = (services.Contains("BARTERS_LOCKPICKS")) ? true : false;
            npc.BARTERS_PROBES = (services.Contains("BARTERS_PROBES")) ? true : false;
            npc.BARTERS_APPARATUS = (services.Contains("BARTERS_APPARATUS")) ? true : false;
            npc.BARTERS_ENCHANTED_ITEMS = (services.Contains("BARTERS_ENCHANTED_ITEMS")) ? true : false;
            npc.OFFERS_SPELLMAKING = (services.Contains("OFFERS_SPELLMAKING")) ? true : false;
            npc.OFFERS_SPELLS = (services.Contains("OFFERS_SPELLS")) ? true : false;
            npc.OFFERS_REPAIRS = (services.Contains("OFFERS_REPAIRS")) ? true : false;
            npc.OFFERS_ENCHANTING = (services.Contains("OFFERS_ENCHANTING")) ? true : false;
            npc.OFFERS_TRAINING = (services.Contains("OFFERS_TRAINING")) ? true : false;

            bool hasAnyService =
                npc.BARTERS_WEAPONS ||
                npc.BARTERS_ARMOR ||
                npc.BARTERS_REPAIR_ITEMS ||
                npc.BARTERS_INGREDIENTS ||
                npc.BARTERS_ALCHEMY ||
                npc.BARTERS_BOOKS ||
                npc.BARTERS_CLOTHING ||
                npc.BARTERS_LIGHTS ||
                npc.BARTERS_MISC_ITEMS ||
                npc.BARTERS_LOCKPICKS ||
                npc.BARTERS_PROBES ||
                npc.BARTERS_APPARATUS ||
                npc.BARTERS_ENCHANTED_ITEMS ||
                npc.OFFERS_SPELLMAKING ||
                npc.OFFERS_SPELLS ||
                npc.OFFERS_REPAIRS ||
                npc.OFFERS_ENCHANTING ||
                npc.OFFERS_TRAINING ||
                npc.OFFERS_TRAVEL;

            npc.No_Services = !hasAnyService;

            return npc;

        }

        public static List<Models.InventoryItem> GetInventory(JsonElement element)
        {
            List<Models.InventoryItem> inventory = new List<Models.InventoryItem>();
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement array in element.EnumerateArray())
                {
                    Models.InventoryItem inventoryItem = new Models.InventoryItem();
                    inventoryItem.Quantity = array[0].GetInt32();
                    inventoryItem.ItemId = array[1].GetString();
                    inventory.Add(inventoryItem);
                }
            }
            return inventory;
        }
        public static List<string> GetSpells(JsonElement element)
        {
            List<string> spells = new List<string>();
            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in element.EnumerateArray())
                {
                    var spell = item.GetString();
                    if (!string.IsNullOrEmpty(spell)) spells.Add(spell);
                }
            }
            return spells;
        }
        public static Models.Skills SetSkills(JsonElement element)
        {
            Models.Skills skills = new Models.Skills();

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

        public static Models.Cell GetCell(JsonElement element)
        {
            Models.Cell cell = new Models.Cell();
            if (element.TryGetProperty("name", out JsonElement name)) cell.CellName = name.GetString();
            if (element.TryGetProperty("data", out JsonElement data)) cell = SetCellFlags(data, cell);
            if (element.TryGetProperty("region", out JsonElement region)) cell.CellRegion = region.GetString();
            if (!string.IsNullOrEmpty(cell.CellName) && cell.CellName.Contains(","))
            {
                cell.Location = cell.CellName.Split(',')[0].Trim();
                cell.SubLocation = cell.CellName.Split(',')[1].Trim();
            }
            else
            {
                cell.Location = cell.CellName;
            }
            if (element.TryGetProperty("references", out JsonElement references)) cell.CellRefs = GetCellRefs(references);
            return cell;
        }
        public static Models.Cell SetCellFlags(JsonElement element, Models.Cell cell)
        {
            string flags = "";
            if (element.TryGetProperty("flags", out JsonElement flag)) flags = flag.GetString() ?? string.Empty;
            cell.IsInterior = (flags.Contains("IS_INTERIOR") && !flags.Contains("BEHAVES_LIKE_EXTERIOR")) ? true : false;
            return cell;
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

        public static void AddCellLocationInfoToNPC(Models.Npc npc, List<Models.Cell> cells)
        {
            foreach (var cell in cells)
            {
                if (cell.CellRefs != null)
                {
                    foreach (string reference in cell.CellRefs)
                    {
                        if (reference == npc.Id)
                        {
                            npc.Region = cell.CellRegion;
                            npc.CellName = cell.CellName;
                            npc.IsInterior = cell.IsInterior;
                            npc.Location = cell.Location;
                            npc.SubLocation = cell.SubLocation;
                            return;
                        }
                    }
                }
            }
        }
        public static void AddExpansionInfoToNPC(Models.Npc npc, List<Models.Expansion> expansions)
        {
            foreach (var expansion in expansions)
            {
                if (expansion.NPCId == npc.Id)
                {
                    npc.Expansion = expansion.Name;
                    return;
                }
            }
        }
    }
}
