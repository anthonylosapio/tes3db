namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static tes3db.Models;

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
                List<FieldValueandType> values = GetPropertyValues(npc, typeof(Models.Npc));
                int c = 0;
                foreach (var value in values)
                {
                    string item = value.Value?.ToString() ?? "";
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
        //Write the start of the INSERT statement with column names
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
        // Write the values for each rows to insert
            counter = 0;
            foreach(var npc in data)
            {
                string queryLine = "(";
                List<FieldValueandType> values = GetPropertyValues(npc, typeof(Models.Npc));
                int c = 0;
                foreach (var value in values) {
                    string field = FormatValueForSql(value);
                    queryLine += field;
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

    private static List<FieldValueandType> GetPropertyValues(object instance, Type type)
    {
        //var propertyValues = new List<object?>();

        var properties = type.GetProperties();

        var fieldValueandType = new List<FieldValueandType>();

        var targetTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int?),
            typeof(bool?),
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
                var newFieldValueandType = new FieldValueandType
                {
                    Value = value,
                    Type = property.PropertyType
                };
                fieldValueandType.Add(newFieldValueandType);
               // propertyValues.Add(value);
            }else if(serializeTypes.Contains(property.PropertyType))
            {
                var value = property.GetValue(instance);
                string serializedValue = JsonSerializer.Serialize(value);
                var newFieldValueandType = new FieldValueandType
                {
                    Value = serializedValue,
                    Type = property.PropertyType
                };
                fieldValueandType.Add(newFieldValueandType);
               // propertyValues.Add(serializedValue);
            }
            else
            {
                var nestedInstance = property.GetValue(instance);
                if (nestedInstance is not null)
                {
                    var nestedValues = GetPropertyValues(nestedInstance, property.PropertyType);
                    fieldValueandType.AddRange(nestedValues);
                }
            }
        }

        return fieldValueandType;
    }

    private static string FormatValueForSql(FieldValueandType obj)
    {
        if(obj.Type == typeof(int?))
        {
            return obj.Value?.ToString() ?? "NULL";
        }
        if (obj.Type == typeof(bool?))
        {
            string returnValue = obj.Value?.ToString() ?? "False";
            return $"'{returnValue}'";
        }

        string s = obj.Value?.ToString() ?? "";
        return $"'{s.Replace("'", "''")}'";
    }

}

