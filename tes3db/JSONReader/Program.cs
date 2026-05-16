using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace tes3db.JSONReader
{
    class Program
    {
        static void Main()
        {
            var sw = Stopwatch.StartNew();

            string outputFileName = "AllNpc_Insert";
            /* only applicable to csv output */
            bool includeColumnHeadings = true;
            /*
             * csv - writes extracted json to csv file
             * sql - writes extracted json to sql insert statements
             */
            string outputFileType = "sql";
            //mysql or postgresql - determines the syntax of the sql insert statements
            string sqlType = "postgresql";
            string tableName = "Npc";

            List<Models.Npc> npcs = new List<Models.Npc>();
            List<Models.Cell> cells = new List<Models.Cell>();
            List<Models.Expansion> expansions = new List<Models.Expansion>();

            var bloodmoonPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "Bloodmoon.json");
            var morrowindPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "morrowind.json");
            var npcPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "npc.json");
            var npcAutoCalcPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "npc_AutoCalcOff.json");
            var tamrielRebuiltPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "TamrielRebuilt.json");
            var tribunalPath = Path.Combine(AppContext.BaseDirectory, "JSONReader", "data", "Tribunal.json");

            /* Get NPC per expansion */
            List<string> expansionsFilePaths = new List<string> { morrowindPath, tribunalPath, bloodmoonPath, tamrielRebuiltPath };
            List<string> expansionNames = new List<string> { "Morrowind", "Tribunal", "Bloodmoon", "Tamriel Rebuilt" };
            int expansionIndex = 0;
            
            Console.WriteLine("Creating NPC - Expansion Map...");
            
            foreach ( string expansionFilePath in expansionsFilePaths)
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
                                if (!expansions.Any(p => p.NPCId == expansion.NPCId)) { expansions.Add(expansion); } 
                                else { Console.WriteLine(expansionNames[expansionIndex] + " " + expansion.NPCId); }
                                
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Expecting an array.");
                }
                Console.WriteLine("Expansion: "+ expansionNames[expansionIndex] + " - found " + expansions.Count + "NPCs");
                expansionIndex++;
            }

            /* Get The NPC Data  */
            List<string> allFilePaths = new List<string> { npcAutoCalcPath, morrowindPath, tribunalPath, bloodmoonPath, tamrielRebuiltPath };
            Console.WriteLine("Extracting NPC & Cell Data...");
            foreach (string path in allFilePaths) {
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
                Console.WriteLine($"After {path} : {npcs.Count} NPCs found, skipped {skipCount} already added, and {cells.Count} Cells ");
            }

            //populate location & expansion informatin of NPCs
            Console.WriteLine("Adding Cell & Region to NPCs (takes a long time)...");
            foreach (var npc in npcs) {
                Functions.AddCellLocationInfoToNPC(npc, cells);
                Functions.AddExpansionInfoToNPC(npc, expansions);
            }

            // List NPCs missing attributes, skills or cell placement
            foreach(var npc in npcs){
                if (npc.CellName == null && npc.Region == null) Console.WriteLine("CellName & Region mssing - "+npc.Id);
                if (npc.Attributes is null) Console.WriteLine($"Attributes missing - {npc.Id}");
                if (npc.Skills == null) Console.WriteLine($"Skills missing - " + npc.Id);
            }

            // Remove objects from list that we don't want to include
            npcs.RemoveAll(item => item.Attributes == null);
            npcs.RemoveAll(item => (item.CellName == null && item.Region == null));

            Console.WriteLine("Writing output file...");

            string outputFile = $"{outputFileName}.{outputFileType}";
            switch (outputFileType.ToLower())
            {
                case "csv":
                    FileWriter.WriteNpcCsv(outputFile, npcs, includeColumnHeadings);
                    break;
                case "sql":
                    FileWriter.WriteNpcSql(outputFile, npcs, tableName, sqlType);
                    break;
                default:
                    Console.WriteLine("Unsupported output file type. Please choose 'csv' or 'sql'.");
                    break;
            }
            sw.Stop();
            Console.WriteLine($"Runtime: {sw.Elapsed}");
        }//end of Main

    }//end of Program

}//end of namespace