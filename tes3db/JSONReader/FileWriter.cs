namespace tes3db.JSONReader;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using static tes3db.JSONReader.Models;

public class FileWriter
{
    public static void WriteNpcCsv(string filePath, List<Models.Npc> data, bool includeColumnHeadings)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");
        if (data == null || data.Count == 0)
            throw new ArgumentException("No data to write.");

        // Use UTF-8 encoding for compatibility
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            // Write header
            if (includeColumnHeadings)
            {
                List<string> cols = GetPropertyNames(typeof(Models.Npc));
                string header = $"{string.Join(", ", cols)}";
                writer.WriteLine(header);
            }

            // Write each record
            foreach (var item in data)
            {
                // Escape commas and quotes if needed
                string id = (string.IsNullOrEmpty(item.Id)) ? string.Empty : EscapeCsvField(item.Id);
                string name = (string.IsNullOrEmpty(item.Name)) ? string.Empty : EscapeCsvField(item.Name);
                string race = (string.IsNullOrEmpty(item.Race)) ? string.Empty : EscapeCsvField(item.Race);
                string npcclass = (string.IsNullOrEmpty(item.Class)) ? string.Empty : EscapeCsvField(item.Class);
                string faction = (string.IsNullOrEmpty(item.Faction)) ? string.Empty : EscapeCsvField(item.Faction);
                string flags = (string.IsNullOrEmpty(item.NpcFlags)) ? string.Empty : EscapeCsvField(item.NpcFlags);
                string gender = (string.IsNullOrEmpty(item.Gender)) ? string.Empty : EscapeCsvField(item.Gender);
                string isessential = EscapeCsvField(item.IsEssential.ToString() ?? "False");
                string ispersistent = EscapeCsvField(item.IsPersistent.ToString() ?? "False");
                string inventory = EscapeCsvField(JsonSerializer.Serialize(item.Inventory));
                string spells = EscapeCsvField(JsonSerializer.Serialize(item.Spells));
                string location = (string.IsNullOrEmpty(item.Location)) ? string.Empty : EscapeCsvField(item.Location);
                string sublocation = (string.IsNullOrEmpty(item.SubLocation)) ? string.Empty : EscapeCsvField(item.SubLocation);
                string cellname = (string.IsNullOrEmpty(item.CellName)) ? string.Empty : EscapeCsvField(item.CellName);
                string region = (string.IsNullOrEmpty(item.Region)) ? string.Empty : EscapeCsvField(item.Region);
                string isinterior = item.IsInterior.ToString() ?? "False";
                string expansion = (string.IsNullOrEmpty(item.Expansion)) ? string.Empty : EscapeCsvField(item.Expansion); ;

                string level = EscapeCsvField(item.Level.ToString() ?? string.Empty);
                string health = EscapeCsvField(item.Health.ToString() ?? string.Empty);
                string magicka = EscapeCsvField(item.Magicka.ToString() ?? string.Empty);
                string fatigue = EscapeCsvField(item.Fatigue.ToString() ?? string.Empty);
                string hello = EscapeCsvField(item.Hello.ToString() ?? string.Empty);
                string fight = EscapeCsvField(item.Fight.ToString() ?? string.Empty);
                string flee = EscapeCsvField(item.Flee.ToString() ?? string.Empty);
                string alarm = EscapeCsvField(item.Alarm.ToString() ?? string.Empty);
                string disposition = EscapeCsvField(item.Disposition.ToString() ?? string.Empty);
                string reputation = EscapeCsvField(item.Reputation.ToString() ?? string.Empty);
                string rank = EscapeCsvField(item.Rank.ToString() ?? string.Empty);
                string gold = EscapeCsvField(item.Gold.ToString() ?? string.Empty);

                string no_services = EscapeCsvField(item.No_Services.ToString() ?? string.Empty);
                string BARTERS_WEAPONS = EscapeCsvField(item.BARTERS_WEAPONS.ToString() ?? string.Empty);
                string BARTERS_ARMOR = EscapeCsvField(item.BARTERS_ARMOR.ToString() ?? string.Empty);
                string BARTERS_REPAIR_ITEMS = EscapeCsvField(item.BARTERS_REPAIR_ITEMS.ToString() ?? string.Empty);
                string BARTERS_INGREDIENTS = EscapeCsvField(item.BARTERS_INGREDIENTS.ToString() ?? string.Empty);
                string BARTERS_ALCHEMY = EscapeCsvField(item.BARTERS_ALCHEMY.ToString() ?? string.Empty);
                string BARTERS_BOOKS = EscapeCsvField(item.BARTERS_BOOKS.ToString() ?? string.Empty);
                string BARTERS_CLOTHING = EscapeCsvField(item.BARTERS_CLOTHING.ToString() ?? string.Empty);
                string BARTERS_LIGHTS = EscapeCsvField(item.BARTERS_LIGHTS.ToString() ?? string.Empty);
                string BARTERS_MISC_ITEMS = EscapeCsvField(item.BARTERS_MISC_ITEMS.ToString() ?? string.Empty);
                string BARTERS_LOCKPICKS = EscapeCsvField(item.BARTERS_LOCKPICKS.ToString() ?? string.Empty);
                string BARTERS_PROBES = EscapeCsvField(item.BARTERS_PROBES.ToString() ?? string.Empty);
                string BARTERS_APPARATUS = EscapeCsvField(item.BARTERS_APPARATUS.ToString() ?? string.Empty);
                string BARTERS_ENCHANTED_ITEMS = EscapeCsvField(item.BARTERS_ENCHANTED_ITEMS.ToString() ?? string.Empty);
                string OFFERS_SPELLMAKING = EscapeCsvField(item.OFFERS_SPELLMAKING.ToString() ?? string.Empty);
                string OFFERS_SPELLS = EscapeCsvField(item.OFFERS_SPELLS.ToString() ?? string.Empty);
                string OFFERS_REPAIRS = EscapeCsvField(item.OFFERS_REPAIRS.ToString() ?? string.Empty);
                string OFFERS_ENCHANTING = EscapeCsvField(item.OFFERS_ENCHANTING.ToString() ?? string.Empty);
                string OFFERS_TRAINING = EscapeCsvField(item.OFFERS_TRAINING.ToString() ?? string.Empty);
                string OFFERS_TRAVEL = EscapeCsvField(item.OFFERS_TRAVEL.ToString() ?? string.Empty);

                string Strength = EscapeCsvField((item.Attributes?.Strength ?? 0).ToString());
                string Intelligence = EscapeCsvField((item.Attributes?.Intelligence ?? 0).ToString());
                string Willpower = EscapeCsvField((item.Attributes?.Willpower ?? 0).ToString());
                string Agility = EscapeCsvField((item.Attributes?.Agility ?? 0).ToString());
                string Speed = EscapeCsvField((item.Attributes?.Speed ?? 0).ToString());
                string Endurance = EscapeCsvField((item.Attributes?.Endurance ?? 0).ToString());
                string Personality = EscapeCsvField((item.Attributes?.Personality ?? 0).ToString());
                string Luck = EscapeCsvField((item.Attributes?.Luck ?? 0).ToString());

                string Acrobatics = EscapeCsvField((item.Skills?.Acrobatics ?? 0).ToString());
                string Alchemy = EscapeCsvField((item.Skills?.Alchemy ?? 0).ToString());
                string Alteration = EscapeCsvField((item.Skills?.Alteration ?? 0).ToString());
                string Armorer = EscapeCsvField((item.Skills?.Armorer ?? 0).ToString());
                string Athletics = EscapeCsvField((item.Skills?.Athletics ?? 0).ToString());
                string Axe = EscapeCsvField((item.Skills?.Axe ?? 0).ToString());
                string Block = EscapeCsvField((item.Skills?.Block ?? 0).ToString());
                string BluntWeapon = EscapeCsvField((item.Skills?.BluntWeapon ?? 0).ToString());
                string Conjuration = EscapeCsvField((item.Skills?.Conjuration ?? 0).ToString());
                string Destruction = EscapeCsvField((item.Skills?.Destruction ?? 0).ToString());
                string Enchant = EscapeCsvField((item.Skills?.Enchant ?? 0).ToString());
                string HandToHand = EscapeCsvField((item.Skills?.HandToHand ?? 0).ToString());
                string HeavyArmor = EscapeCsvField((item.Skills?.HeavyArmor ?? 0).ToString());
                string Illusion = EscapeCsvField((item.Skills?.Illusion ?? 0).ToString());
                string LightArmor = EscapeCsvField((item.Skills?.LightArmor ?? 0).ToString());
                string LongBlade = EscapeCsvField((item.Skills?.LongBlade ?? 0).ToString());
                string Marksman = EscapeCsvField((item.Skills?.Marksman ?? 0).ToString());
                string MediumArmor = EscapeCsvField((item.Skills?.MediumArmor ?? 0).ToString());
                string Mercantile = EscapeCsvField((item.Skills?.Mercantile ?? 0).ToString());
                string Mysticism = EscapeCsvField((item.Skills?.Mysticism ?? 0).ToString());
                string Restoration = EscapeCsvField((item.Skills?.Restoration ?? 0).ToString());
                string Security = EscapeCsvField((item.Skills?.Security ?? 0).ToString());
                string ShortBlade = EscapeCsvField((item.Skills?.ShortBlade ?? 0).ToString());
                string Sneak = EscapeCsvField((item.Skills?.Sneak ?? 0).ToString());
                string Spear = EscapeCsvField((item.Skills?.Spear ?? 0).ToString());
                string Speechcraft = EscapeCsvField((item.Skills?.Speechcraft ?? 0).ToString());
                string Unarmored = EscapeCsvField((item.Skills?.Unarmored ?? 0).ToString());

                if (!String.IsNullOrEmpty(cellname) || !String.IsNullOrEmpty(region))
                {
                    string line = $"{id},{name},{race},{npcclass},{faction},{flags},{gender},{isessential},{ispersistent},{inventory},{spells},";
                    line += $"{location},{sublocation},{cellname},{region},{isinterior},{expansion},";
                    line += $"{level},{health},{magicka},{fatigue},{hello},{fight},{flee},{alarm},{disposition},{reputation},{rank},{gold},";
                    line += $"{no_services},{BARTERS_WEAPONS},{BARTERS_ARMOR},{BARTERS_REPAIR_ITEMS},{BARTERS_INGREDIENTS},{BARTERS_ALCHEMY},{BARTERS_BOOKS},{BARTERS_CLOTHING},{BARTERS_LIGHTS},{BARTERS_MISC_ITEMS},{BARTERS_LOCKPICKS},{BARTERS_PROBES},{BARTERS_APPARATUS},{BARTERS_ENCHANTED_ITEMS},{OFFERS_SPELLMAKING},{OFFERS_SPELLS},{OFFERS_REPAIRS},{OFFERS_ENCHANTING},{OFFERS_TRAINING},{OFFERS_TRAVEL},";
                    line += $"{Strength},{Intelligence},{Willpower},{Agility},{Speed},{Endurance},{Personality},{Luck},";
                    line += $"{Acrobatics},{Alchemy},{Alteration},{Armorer},{Athletics},{Axe},{Block},{BluntWeapon},{Conjuration},{Destruction},{Enchant},{HandToHand},{HeavyArmor},{Illusion},{LightArmor},{LongBlade},{Marksman},{MediumArmor},{Mercantile},{Mysticism},{Restoration},{Security},{ShortBlade},{Sneak},{Spear},{Speechcraft},{Unarmored}";
                    writer.WriteLine(line);
                }
            }
        }
    }

    public static void WriteNpcSql(string filePath, List<Models.Npc> data, string tableName, string sqlType)
    {
        List<string> cols = GetPropertyNames(typeof(Models.Npc));
        int counter =0;

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");
        
        string q = (sqlType=="postgresql") ? "\"" : "`";
        string queryStart = $"INSERT INTO {tableName} (";
        foreach(string col in cols)
        {
            queryStart+= $"{q}{col}{q}";
            if(counter < cols.Count - 1) queryStart += ", ";
            counter++;
        }
        queryStart += ") VALUES ";
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine(queryStart);
            counter = 0;
            foreach(var npc in data)
            {
                string queryLine = "(";
                List<object?> values = GetPropertyValues(npc, typeof(Models.Npc));
                int c = 0;
                foreach (var value in values) {
                    string item = value?.ToString() ?? "";
                    queryLine += $"'{item.Replace("'", "''")}'";
                    if(c < values.Count - 1) queryLine += ",";
                    c++;
                }
                queryLine += ")";
                
                if(counter < data.Count - 1) queryLine += ",";
                
                writer.WriteLine(queryLine);
                counter++;
            }

        }
    }
    private static string EscapeCsvField(string field)
    {
        if (field == null) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            field = field.Replace("\"", "\"\""); // Escape quotes
            return $"\"{field}\""; //Wrap in quotes 
        }
        return field;
    }


    private static List<string> GetPropertyNames(Type type)
    {
        var propertyNames = new List<string>();
        var properties = type.GetProperties();

        var targetTypes = new HashSet<Type>
            {
                typeof(string),
                typeof(int?),
                typeof(bool?),
                typeof(List<InventoryItem>),
                typeof(List<string>)
            };

        foreach (var property in properties)
        {
            string propertyName = property.Name;
            if (targetTypes.Contains(property.PropertyType))
            {
                propertyNames.Add(propertyName);
            }
            else
            {
                var nestedProperties = GetPropertyNames(property.PropertyType);
                propertyNames.AddRange(nestedProperties);
            }

        }

        return propertyNames;
    }

    private static List<object?> GetPropertyValues(object instance, Type type)
    {
        var propertyValues = new List<object?>();
        var properties = type.GetProperties();

        var targetTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int?),
            typeof(bool),
        };

        var serializeTypes = new HashSet<Type>
        {
            typeof(List<InventoryItem>),
            typeof(List<string>)
        };

        foreach (var property in properties)
        {
            if (targetTypes.Contains(property.PropertyType))
            {
                var value = property.GetValue(instance);
                propertyValues.Add(value);
            }else if(serializeTypes.Contains(property.PropertyType))
            {
                var value = property.GetValue(instance);
                string serializedValue = JsonSerializer.Serialize(value);
                propertyValues.Add(serializedValue);
            }
            else
            {
                var nestedInstance = property.GetValue(instance);
                if (nestedInstance is not null)
                {
                    var nestedValues = GetPropertyValues(nestedInstance, property.PropertyType);
                    propertyValues.AddRange(nestedValues);
                }
            }
        }

        return propertyValues;
    }
}

