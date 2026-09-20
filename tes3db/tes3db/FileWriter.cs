namespace tes3db;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static tes3db.Models;
using static tes3db.Models.Faction;
using static tes3db.Models.Faction.FactionData;
using static tes3db.Models.Npc;

public class FileWriter
{
    public static void WriteCsv<T>(string filePath, List<T> data, bool includeColumnHeadings, string format)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");
        if (data == null || data.Count == 0) {
            Console.WriteLine($"Skipping {filePath}. No data to write.");
            return;
        }

        char delimiter = format.ToLowerInvariant() switch
        {
            "csv" => ',',
            "tsv" => '\t',
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        // Use UTF-8 encoding for compatibility
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            // Write header
            if (includeColumnHeadings)
            {
                List<PropertyNameandType> cols = GetPropertyNames(typeof(T));
                string header = $"{string.Join(delimiter, cols.Select(c => c.Name))}";
                writer.WriteLine(header);
            }
            // Write each record
            foreach (var obj in data)
            {
                List<FieldValueandType> values = GetPropertyValues(obj, typeof(T));
                var line = new StringBuilder();

                int c = 0;
                foreach (var value in values)
                {
                    string item = value.Value?.ToString() ?? "";
                    line.Append(EscapeField(item, delimiter));
                    if (c < values.Count - 1) line.Append(delimiter);
                    c++;
                }
                writer.WriteLine(line);
            }
        }
    }

    public static void WriteSql<T>(string filePath, List<T> data, string tableName, string sqlType)
    {
        List<PropertyNameandType> cols = GetPropertyNames(typeof(T));
        int counter = 0;

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");
        if (data == null || data.Count == 0)
        {
            Console.WriteLine($"Skipping {filePath}. No data to write.");
            return;
        }

        string q = (sqlType == "postgres") ? "\"" : "`";
        //Write the start of the INSERT statement with column names
        string queryStart = $"INSERT INTO {q}{tableName}{q} (";
        foreach (var col in cols)
        {
            queryStart += $"{q}{col.Name}{q}";
            if (counter < cols.Count - 1) queryStart += ", ";
            counter++;
        }
        queryStart += ") VALUES ";

        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine(queryStart);
            // Write the values for each rows to insert
            counter = 0;
            foreach (var obj in data)
            {
                string queryLine = "(";
                List<FieldValueandType> values = GetPropertyValues(obj, typeof(T));
                int c = 0;
                foreach (var value in values)
                {
                    string field = FormatValueForSql(value);
                    queryLine += field;
                    if (c < values.Count - 1) queryLine += ",";
                    c++;
                }
                queryLine += ")";

                if (counter < data.Count - 1) queryLine += ",";

                writer.WriteLine(queryLine);
                counter++;
            }

        }
    }

    private static string EscapeField(string field, char delimiter)
    {
        if (field.Contains(delimiter) || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }

    /// <summary>
    /// Retrieves property names from the passed in model
    /// </summary>
    /// <param name="type">The type of the model. typeof(T).</param>
    /// <returns>The property names of the passed-in model as a list of strings.</returns>
    private static List<PropertyNameandType> GetPropertyNames(Type type)
    {
        var propertyNameandType = new List<PropertyNameandType>();
        var properties = type.GetProperties();

        var targetTypes = new HashSet<Type>
            {
                typeof(string),
                typeof(int?),
                typeof(bool?),
                typeof(double?),
                typeof(int?[]),
                typeof(string[]),
                typeof(List<InventoryItem>),
                typeof(List<Effect>),
                typeof(List<string>),
                typeof(List<Reaction>),
                typeof(List<Requirement>),
                typeof(List<TravelDestination>),
            };
        var ignoreTypes = new HashSet<Type>
            {
                typeof(JsonElement),
            };

        foreach (var property in properties)
        {
            if (!ignoreTypes.Contains(property.PropertyType)) {
                string propertyName = property.Name;
                if (propertyName == "classs") propertyName = "class";
                if (targetTypes.Contains(property.PropertyType))
                {
                    propertyNameandType.Add(new PropertyNameandType { Name = propertyName.ToLowerInvariant(), Type = property.PropertyType });
                }
                else
                {
                    var nestedProperties = GetPropertyNames(property.PropertyType);
                    propertyNameandType.AddRange(nestedProperties);
                }
            }

        }

        return propertyNameandType;
    }

    private static List<FieldValueandType> GetPropertyValues(object instance, Type type)
    {

        var properties = type.GetProperties();

        var fieldValueandType = new List<FieldValueandType>();

        var targetTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(int?),
            typeof(bool?),
            typeof(double?)
        };
        //Types that will be started as serialized JSON strings in the SQL output
        var serializeTypes = new HashSet<Type>
        {
            typeof(List<InventoryItem>),
            typeof(List<Effect>),
            typeof(List<string>),
            typeof(string[]),
            typeof(int?[]),
            typeof(List<Reaction>),
            typeof(List<Requirement>),
            typeof(List<TravelDestination>),
        };

        var ignoreTypes = new HashSet<Type>
            {
                typeof(JsonElement),
            };

        foreach (var property in properties)
        {
            if (!ignoreTypes.Contains(property.PropertyType))
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
                }
                else if (serializeTypes.Contains(property.PropertyType))
                {
                    var value = property.GetValue(instance);
                    string serializedValue = JsonSerializer.Serialize(value);
                    var newFieldValueandType = new FieldValueandType
                    {
                        Value = serializedValue.Replace("\\u0027", "'"),// Replace escaped single quotes with actual single quotes
                        Type = property.PropertyType
                    };
                    fieldValueandType.Add(newFieldValueandType);
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

    private static string TypeToSql(Type t, string sqlType, int max)
    {
        switch (sqlType.ToLowerInvariant())
        {
            case "mysql":
                if (t == typeof(int?) || t == typeof(long)) { 
                    if(max < 128) return "TINYINT";
                    if (max < 32768) return "SMALLINT";
                    return "INT"; 
                }
                if (t == typeof(bool?)) return "BOOL";
                if (t == typeof(double?) || t == typeof(float)) return "DOUBLE";
                if (t == typeof(string))
                {
                    if(max < 32000) return $"VARCHAR({max})";
                    return "TEXT";
                }
                return "TEXT";
            case "postgres":
                if (t == typeof(int?) || t == typeof(long)) return "BIGINT";
                if (t == typeof(bool?)) return "BOOLEAN";
                if (t == typeof(double?) || t == typeof(float)) return "DOUBLE PRECISION";
                if (t == typeof(string)) return "VARCHAR(255)"; // will be updated later
                return "TEXT";
            case "sqlite":
                if (t == typeof(int?) || t == typeof(long)) return "INTEGER";
                if (t == typeof(bool?)) return "INTEGER"; // no native BOOLEAN; 0/1 convention
                if (t == typeof(double?) || t == typeof(float)) return "REAL";
                if (t == typeof(string)) return "TEXT";
                return "TEXT";
            default:
                throw new ArgumentException($"Unsupported SQL type: {sqlType}");
        }
        
    }

    public static void WriteSqlCreateTableFile(string filePath, object[] listsObject, string[] tableNames, string sqlType, string dbName)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.");

        if (string.IsNullOrWhiteSpace(dbName))
            throw new ArgumentException("Database name cannot be empty.");

        string q = (sqlType == "mysql") ? "`" : "";

        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            //create the database if it doesn't exist, and use it
            if (sqlType == "mysql")
            {
                writer.WriteLine($"CREATE DATABASE IF NOT EXISTS `{dbName}`;");
                writer.WriteLine($"USE `{dbName}`;");
                writer.WriteLine();
            }
            else if (sqlType == "postgres")
            {
                writer.WriteLine($"CREATE DATABASE \"{dbName}\";");
                writer.WriteLine($"\\c \"{dbName}\";");
                writer.WriteLine();
            }
            //begin iterating through the list objects
            foreach (var list in listsObject)
            {

                //iterate through the values of each object and store the maximum length/size of each property
                //Type type = list.GetType();
                Type t = list.GetType().GetGenericArguments()[0];
                List<int> max = new List<int>();

                foreach (var obj in (IEnumerable)list)
                {
                    List<FieldValueandType> values = GetPropertyValues(obj, t);
                    int j = 0;
                    foreach (var value in values)
                    {
                        switch (value.Value)
                        {
                            case string s:
                                int length = s.Length;
                                if (max.Count > j)
                                {
                                    if (max[j] < length)
                                    {
                                        max[j] = length;
                                    }
                                }
                                else
                                {
                                    max.Add(length);
                                }
                                break;
                            case int n:
                                if (max.Count > j)
                                {
                                    if (max[j] < n)
                                    {
                                        max[j] = n;
                                    }
                                }
                                else
                                {
                                    max.Add(n);
                                }
                                break;
                            case bool b:
                            case double d:
                                if (max.Count <= j)
                                {
                                    max.Add(0);
                                }
                                break;
                            default:
                                if (max.Count <= j)
                                {
                                    max.Add(1);
                                }
                                break;
                        }
                        j++;
                    }
                }

                List<PropertyNameandType> cols = GetPropertyNames(t);

                bool hasIdColumn = false;
                int i = 0;
                int rowLength = cols.Count;

                string start = $"CREATE TABLE `{tableNames[Array.IndexOf(listsObject, list)]}` (";
                writer.WriteLine(start);

                foreach (var col in cols)
                {
                    string comma = (i < rowLength - 1) ? "," : "";
                    string idCollation = "";
                    if (col.Name == "id")
                    {
                        hasIdColumn = true;
                        idCollation = " CHARACTER SET utf8mb4 COLLATE utf8mb4_bin";
                        if (sqlType == "sqlite") idCollation = " PRIMARY KEY";
                    }
                    if (hasIdColumn) comma = ",";//include a comma because the PRIMARY KEY line will be added after this line
                    string output = $"{q}{col.Name}{q} {TypeToSql(col.Type, sqlType, max[i])}{idCollation}{comma}";
                    writer.WriteLine(output);
                    i++;
                }

                if (hasIdColumn && sqlType != "sqlite")
                {
                    writer.WriteLine($"PRIMARY KEY ({q}id{q})");
                }
                string closing = (sqlType == "mysql") ? ") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;" : ");";
                writer.WriteLine(closing);
                writer.WriteLine();
            }

            if (sqlType == "mysql" || sqlType == "sqlite")
            {

                writer.WriteLine("CREATE INDEX `idx_dialogueinfo_next_id` ON `dialogueinfo` (`next_id`, `dialogue_topic`);");
                writer.WriteLine("CREATE INDEX `idx_dialogueinfo_topic_id` ON `dialogueinfo` (`dialogue_topic`, `id`);");
                writer.WriteLine("CREATE INDEX `idx_dialogueinfo_speaker_id` ON `dialogueinfo` (`speaker_id`);");
                writer.WriteLine("CREATE INDEX `idx_dialogueinfo_id_speaker_id` ON `dialogueinfo` (`id`, `speaker_id`);");
            }
        }
    }

}

