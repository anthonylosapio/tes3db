namespace tes3db;

using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();

        string dbName = "tes3db.db";
        string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tes3db_sql");
        string dbFilePath = Path.Combine(outputDirectory, dbName);
        if (!Directory.Exists(outputDirectory)) Directory.CreateDirectory(outputDirectory);

        /* Default parameter values */
        string prefix = "";

        bool refresh = false;

        bool dbExists = File.Exists(dbFilePath) ? true : false;

        // includeColumnHeadings
        // true/false
        // only applicable to csv/tsv output
        bool includeColumnHeadings = true;

        // outputFormat
        // string
        // csv tsv mysql postgres
        string outputFormat = "sqlite";
        string fileExtension = "sql";
        
        // noSkip
        // true/false
        // if false, NPCs missing Cell, Region, Attribute or Skill poperties will be excluded from output
        bool noSkip = true;

        // verbose
        // true/false
        // if true, more status updates are written to the console while the process runs
        bool verbose = false;

        // Parse command-line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--refresh":
                case "-r":
                    refresh = true;
                    break;

                case "--format":
                case "-f":
                    if (i + 1 < args.Length)
                        outputFormat = args[++i];
                    break;

                case "--skip":
                    noSkip = false;
                    break;

                case "--no-headers":
                    includeColumnHeadings = false;
                    break;

                case "--verbose":
                    verbose = true;
                    break;

                case "--prefix":
                case "-p":
                    if (i + 1 < args.Length)
                        prefix = args[++i];
                    break;

                case "--help":
                case "-help":
                    PrintUsage();
                    return;
            }
        }
        fileExtension = outputFormat.ToLowerInvariant() switch
        {
            "csv" => "csv",
            "tsv" => "tsv",
            "mysql" => "sql",
            "postgres" => "sql",
            "sqlite" => "sql",
            _ => throw new ArgumentException($"Unknown output format: {outputFormat}")
        };

        if((!dbExists || outputFormat != "sqlite") || refresh )
        {
            FileReader.Main(verbose, noSkip, includeColumnHeadings, prefix, fileExtension, outputDirectory, outputFormat);
        }

        //Do specific SQLITE TASKS HERE - create the database if it doesn't exist and run the newly generated import scripts
        if (outputFormat == "sqlite") { 
            
            if (dbExists && refresh)
            {
                if (verbose) Console.WriteLine($"Removing existing SQLite database file at: {dbFilePath}");
                File.Delete(dbFilePath);
                dbExists = false;
            }
        
            if(!dbExists)
            {
                if(verbose) Console.WriteLine($"Creating SQLite database at: {dbFilePath}");
                // Create the SQLite database
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbFilePath}"))
                {
                    connection.Open();
                    // Execute the SQL script to create tables and insert data
                    string sqlScriptPath = Path.Combine(outputDirectory, $"tes3db.sql");
                    if (File.Exists(sqlScriptPath))
                    {
                        string sqlScript = File.ReadAllText(sqlScriptPath);

                        string[] statements = sqlScript.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var statement in statements)
                        {
                            string trimmedStatement = statement.Trim();
                            if (!string.IsNullOrEmpty(trimmedStatement))
                            {
                                using (var command = connection.CreateCommand())
                                {
                                    command.CommandText = trimmedStatement;
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"SQL script file not found: {sqlScriptPath}");
                    }

                    //Populate the database with data from the generated sql file
                    var sqlFiles = Directory.GetFiles(outputDirectory, "*.sql")
                        .Where(f => !Path.GetFileName(f).Equals("tes3db.sql", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    foreach (var sqlFile in sqlFiles)
                    {
                        Console.WriteLine($"Executing {Path.GetFileName(sqlFile)}...");

                        string sqlScript = File.ReadAllText(sqlFile);
                        if (!string.IsNullOrEmpty(sqlScript))
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlScript;
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                }
            }
        }
        
        sw.Stop();
        Console.WriteLine($"Runtime: {sw.Elapsed}");
    }//end of Main
    static void PrintUsage()
    {
        Console.WriteLine("Usage: tes3db.exe [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --refresh, -r            Force refresh of sqlite database");
        Console.WriteLine("  --format, -f <type>      Output type: sqlite csv tsv mysql postgres (default: sqlite)");
        Console.WriteLine("  --no-headers             Exclude column headers in (csv/tsv only)");
        Console.WriteLine("  --skip                   Will skip NPCs missing Cell, Region, Attribute or Skill poperties");
        Console.WriteLine("  --verbose                Display verbose output");
        Console.WriteLine("");
        Console.WriteLine("  --prefix, -p <prefix>    Prefix for output files, helpful if keeping expansions separate (default: none)");
        Console.WriteLine("");
        Console.WriteLine("  --help                   Show this help message");
    }
}//end of Program
