namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
            foreach (var npc in data)
            {

                string line = string.Empty;
                List<object?> values = GetPropertyValues(npc, typeof(Models.Npc));
                int c = 0;
                foreach (var value in values)
                {
                    string item = value?.ToString() ?? "";
                    line += EscapeCsvField(item);
                    if (c < values.Count - 1) line += ",";
                    c++;
                }
                writer.WriteLine(line);
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
        
        string queryStart = $"INSERT INTO {q}{tableName}{q} (";
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
                typeof(List<Models.InventoryItem>),
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
            typeof(bool?),
        };

        var serializeTypes = new HashSet<Type>
        {
            typeof(List<Models.InventoryItem>),
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

