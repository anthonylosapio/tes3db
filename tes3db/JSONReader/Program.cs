using System.Text;
using System.Text.Json;
using System.Diagnostics;
using static MorrowindNPCExtractor.dataextractor.Models;

namespace MorrowindNPCExtractor.dataextractor
{
    class Program
    {
        static void Main()
        {
            var sw = Stopwatch.StartNew();

            bool includeColumnHeadings = false;
            string outputFileName = "AllNpc_NoHeadings.csv";

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

            // Remove objects from list that we con't want to include
            npcs.RemoveAll(item => item.Attributes == null);
            npcs.RemoveAll(item => (item.CellName == null && item.Region == null));

            Console.WriteLine("Writing output file...");

            WriteNpcCsv(outputFileName, npcs, includeColumnHeadings);
            sw.Stop();
            Console.WriteLine($"Runtime: {sw.Elapsed}");
        }//end of Main



        static void WriteNpcCsv(string filePath, List<Npc> data, bool includeColumnHeadings)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty.");

            if (data == null || data.Count == 0)
                throw new ArgumentException("No data to write.");

            // Use UTF-8 encoding for compatibility
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                if (includeColumnHeadings) {
                    string header = "Id,Name,Race,Class,Faction,Flags,Gender,IsEssential,IsPersistent,Inventory,Spells,";
                    header += "Location,SubLocation,CellName,Region,IsInterior,Expansion,";
                    header += "Level,Health,Magicka,Fatigue,Hello,Fight,Flee,Alarm,Disposition,Reputation,Rank,Gold,";
                    header += "No_Services,BARTERS_WEAPONS,BARTERS_ARMOR,BARTERS_REPAIR_ITEMS,BARTERS_INGREDIENTS,BARTERS_ALCHEMY,BARTERS_BOOKS,";
                    header += "BARTERS_CLOTHING,BARTERS_LIGHTS,BARTERS_MISC_ITEMS,BARTERS_LOCKPICKS,BARTERS_PROBES,BARTERS_APPARATUS,";
                    header += "BARTERS_ENCHANTED_ITEMS,OFFERS_SPELLMAKING,OFFERS_SPELLS,OFFERS_REPAIRS,OFFERS_ENCHANTING,OFFERS_TRAINING,OFFERS_TRAVEL,";
                    header += "Strength,Intelligence,Willpower,Agility,Speed,Endurance,Personality,Luck,";
                    header += "Acrobatics,Alchemy,Alteration,Armorer,Athletics,Axe,Block,BluntWeapon,Conjuration,Destruction,Enchant,";
                    header += "HandToHand,HeavyArmor,Illusion,LightArmor,LongBlade,Marksman,MediumArmor,Mercantile,Mysticism,";
                    header += "Restoration,Security,ShortBlade,Sneak,Spear,Speechcraft,Unarmored";

                    writer.WriteLine(header);
                }
                // Write header


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

                    string Strength = EscapeCsvField( (item.Attributes?.Strength ?? 0).ToString() );
                    string Intelligence = EscapeCsvField( (item.Attributes?.Intelligence ?? 0).ToString() );
                    string Willpower = EscapeCsvField( (item.Attributes?.Willpower ??0).ToString() );
                    string Agility = EscapeCsvField( (item.Attributes?.Agility ?? 0).ToString() );
                    string Speed = EscapeCsvField( (item.Attributes?.Speed ?? 0).ToString() );
                    string Endurance = EscapeCsvField( (item.Attributes?.Endurance ?? 0).ToString() );
                    string Personality = EscapeCsvField( (item.Attributes?.Personality ?? 0).ToString());
                    string Luck = EscapeCsvField( (item.Attributes?.Luck ?? 0).ToString() );

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

                    //writer.WriteLine($"{name},{id},{race},{npcclass},{faction},{location},{sublocation},{region},{gold}");
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

        static string EscapeCsvField(string field)
        {
            if (field == null) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                field = field.Replace("\"", "\"\""); // Escape quotes
                return $"\"{field}\""; //Wrap in quotes 
            }
            return field;
        }


    }//end of Program

}