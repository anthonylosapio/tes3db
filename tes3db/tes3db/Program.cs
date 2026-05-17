namespace tes3db;

using System.Diagnostics;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();

        // Default values
        string outputNpc = "npc";
        /* only applicable to csv output */
        bool includeColumnHeadings = true;
        //sql or csv
        string outputFileType = "csv";
        //mysql or postgresql
        string sqlType = "postgresql";
        bool noSkip = false; //if true, won't skip NPCs missing Cell,Region,Attribute or Skill poperties, and will include them in output with null values for missing properties

        // Parse command-line arguments
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--npc":
                    if (i + 1 < args.Length)
                        outputNpc = args[++i];
                    break;

                case "--type":
                case "-t":
                    if (i + 1 < args.Length)
                        outputFileType = args[++i];
                    break;

                case "--sql-type":
                case "-s":
                    if (i + 1 < args.Length)
                        sqlType = args[++i];
                    break;

                case "--no-skip":
                    noSkip = true;
                    break;

                case "--no-headers":
                    includeColumnHeadings = false;
                    break;

                case "--help":
                    PrintUsage();
                    return;
            }
        }

        List<Models.Npc> npcs = new List<Models.Npc>();
        List<Models.Cell> cells = new List<Models.Cell>();
        List<Models.Expansion> expansions = new List<Models.Expansion>(); //tracks which JSON file an NPC came from

        /*Get List of JSON files in the executable directory*/
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        List<string> jsonFilePaths = Directory.GetFiles(currentDir, "*.json").ToList();
        List<string> expansionFilePaths = new List<string>();

        if (jsonFilePaths.Count == 0)
        {
            Console.WriteLine("No JSON files found. Please ensure the JSON files are in the same directory as the executable.");
            Environment.Exit(0);
        }
        else
        {
            foreach (string file in jsonFilePaths) { Console.WriteLine("Found JSON file: " + file); }
        }

        //Get the file names with the path or extension, these will be used to populate the "Expansion" column
        List<string> expansionNames = new List<string>();
        foreach (string file in jsonFilePaths)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string stripped = fileNameWithoutExt.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            expansionNames.Add(stripped);

        }
        /*Check for special npc.json file
         npc.json file can be generated if you need attributes & skills from npc's with autocalculate on.
        To generate an npc.json file first create an npc.esm file by opening the .esp/.esm files(s) in the construction
        set and toggleing autocalculate off for those npc. Save the result as npc.esm then run tes3conv to convert to
        npc.json. If found, npc.json by default will be read before any other json files, but will be excluded from
        Expansion column assignment */
        bool hasNpcJson = (expansionNames.Contains("npc")) ? true : false;
        string npcJsonPath = string.Empty;
        if (hasNpcJson)
        {
            Console.WriteLine("npc.json file found.");
            npcJsonPath = Path.Combine(currentDir, "npc.json");
            jsonFilePaths.Remove(npcJsonPath);
            expansionNames.Remove("npc");
            expansionFilePaths.AddRange(jsonFilePaths);
            jsonFilePaths.Insert(0, npcJsonPath);
        }
        else
        {
            Console.WriteLine("no npc.json file found.");
        }

        int expansionIndex = 0;

        Console.WriteLine("Creating NPC - Expansion Map...");

        foreach (string expansionFilePath in expansionFilePaths)
        {
            using FileStream expansionFs = File.OpenRead(expansionFilePath);
            using JsonDocument expansionDoc = JsonDocument.Parse(expansionFs);
            JsonElement expansionRoot = expansionDoc.RootElement;
            if (expansionRoot.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in expansionRoot.EnumerateArray())
                {
                    if (element.TryGetProperty("type", out JsonElement type))
                    {
                        if (type.GetString() == "Npc")
                        {
                            Models.Expansion expansion = Functions.SetExpansion(expansionNames[expansionIndex], element);
                            if (!expansions.Any(p => p.NPCId == expansion.NPCId))
                            {
                                expansions.Add(expansion);
                            }
                            else
                            {
                                string alreadyAddedTo = expansions.Find(x => x.NPCId == expansion.NPCId)?.Name ?? "";
                                Console.WriteLine(expansionNames[expansionIndex] + " " + expansion.NPCId + " already added to " + alreadyAddedTo);
                            }

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Expecting an array.");
            }
            Console.WriteLine("Expansion: " + expansionNames[expansionIndex] + " - found " + expansions.Count + "NPCs");
            expansionIndex++;
        }

        /* Get The NPC Data  */
        Console.WriteLine("Extracting NPC & Cell Data...");

        foreach (string path in jsonFilePaths)
        {
            using FileStream fs = File.OpenRead(path);
            using JsonDocument doc = JsonDocument.Parse(fs);
            JsonElement root = doc.RootElement;

            int skipCount = 0;

            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement element in root.EnumerateArray())
                {
                    if (element.TryGetProperty("type", out JsonElement type))
                    {
                        if (type.GetString() == "Npc")
                        {
                            Models.Npc npc = Functions.newNpc(element);
                            if (!npcs.Any(p => p.Id == npc.Id)) { npcs.Add(npc); } else { skipCount++; }
                        }
                        if (type.GetString() == "Cell")
                        {
                            Models.Cell cell = Functions.GetCell(element);
                            cells.Add(cell);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Expecting an array.");
            }
            Console.WriteLine($"After {Path.GetFileName(path)} : {npcs.Count} NPCs found, skipped {skipCount} already added, and {cells.Count} Cells ");
        }

        //populate location & expansion informatin of NPCs
        Console.WriteLine("Adding Cell & Region to NPCs (takes a long time)...");
        foreach (var npc in npcs)
        {
            Functions.AddCellLocationInfoToNPC(npc, cells);
            Functions.AddExpansionInfoToNPC(npc, expansions);
        }

        // List NPCs missing attributes, skills or cell placement
        if(!noSkip)
        {
            foreach (var npc in npcs)
            {
                if (npc.CellName == null && npc.Region == null) Console.WriteLine("CellName & Region mssing - " + npc.Id);
                if (npc.Attributes is null) Console.WriteLine($"Attributes missing - {npc.Id}");
                if (npc.Skills == null) Console.WriteLine($"Skills missing - " + npc.Id);
            }

            // Remove objects from list that we don't want to include
            npcs.RemoveAll(item => item.Attributes == null);
            npcs.RemoveAll(item => (item.CellName == null && item.Region == null));
        }

        Console.WriteLine("Writing output file...");

        string outputFile = $"{outputNpc}.{outputFileType}";
        switch (outputFileType.ToLower())
        {
            case "csv":
                FileWriter.WriteNpcCsv(outputFile, npcs, includeColumnHeadings);
                break;
            case "sql":
                FileWriter.WriteNpcSql(outputFile, npcs, outputNpc, sqlType);
                break;
            default:
                Console.WriteLine("Unsupported output file type. Please choose 'csv' or 'sql'.");
                break;
        }
        sw.Stop();
        Console.WriteLine($"Runtime: {sw.Elapsed}");
    }//end of Main
    static void PrintUsage()
    {
        Console.WriteLine("Usage: tes3db.exe [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --type, -t <type>        Output type: csv or sql (default: csv)");
        Console.WriteLine("  --sql-type, -s <type>    SQL type: mysql or postgresql (default: postgresql)");
        Console.WriteLine("  --npc <name>             db Table & output File name for extracted NPCs (default: npc)");
        Console.WriteLine("  --no-headers             Exclude column headers in (CSV only)");
        Console.WriteLine("  --no-skip                Wont's skip NPCs missing Cell,Region,Attribute or Skill poperties");
        Console.WriteLine("  --help                   Show this help message");
    }
}//end of Program
