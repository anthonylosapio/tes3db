namespace tes3db;

using System.Diagnostics;
using System.Text.Json;
using static tes3db.Models;

class Program
{
    static void Main(string[] args)
    {
        var sw = Stopwatch.StartNew();

        /* Default parameter values */
        // filename/table values
        string outputAlchemy = "alchemy";
        string outputApparatus = "apparatus";
        string outputArmor = "armor";
        string outputBirthsign = "birthsign";
        string outputBook = "book";
        string outputClass = "class";
        string outputClothing = "clothing";
        string outputCreature = "creature";
        string outputDialogue = "dialogue";
        string outputDialogueInfo = "dialogueinfo";
        string outputEffect = "effect";
        string outputEnchanting = "enchanting";
        string outputFaction = "faction";
        string outputIngredient = "ingredient";
        string outputLockpick = "lockpick";
        string outputMiscItem = "miscitem";
        string outputNpc = "npc";
        string outputProbe = "probe";
        string outputRace = "race";
        string outputSkill = "skill";
        string outputSpell = "spell";
        string outputWeapon = "weapon";

        string prefix = "";

        // includeColumnHeadings
        // true/false
        // only applicable to csv/tsv output
        bool includeColumnHeadings = true;

        // outputFormat
        // string
        // csv tsv mysql postgres
        string outputFormat = "csv";
        string fileExtension = "csv";
        
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
                case "--alchemy":
                    if (i + 1 < args.Length)
                        outputAlchemy = args[++i];
                    break;  

                case "--apparatus":
                    if (i + 1 < args.Length)
                        outputApparatus = args[++i];
                    break;

                case "--armor":
                    if (i + 1 < args.Length)
                        outputArmor = args[++i];
                    break;

                case "--birthsign":
                    if (i + 1 < args.Length)
                        outputBirthsign = args[++i];
                    break;
                
                case "--book":
                    if (i + 1 < args.Length)
                        outputBook = args[++i];
                    break;

                case "--class":
                    if (i + 1 < args.Length)
                        outputClass = args[++i];
                    break;

                case "--clothing":
                    if (i + 1 < args.Length)
                        outputClothing = args[++i];
                    break;

                case "--creature":
                    if (i + 1 < args.Length)
                        outputCreature = args[++i];
                    break;

                case "--dialogue":
                    if (i + 1 < args.Length)
                        outputDialogue = args[++i];
                    break;

                case "--dialogueinfo":
                    if (i + 1 < args.Length)
                        outputDialogueInfo = args[++i];
                    break;

                case "--effect":
                    if (i + 1 < args.Length)
                        outputEffect = args[++i];
                    break;

                case "--faction":
                    if (i + 1 < args.Length)
                        outputFaction = args[++i];
                    break;

                case "--ingredient":
                    if (i + 1 < args.Length)
                        outputIngredient = args[++i];
                    break;

                case "--lockpick":
                    if (i + 1 < args.Length)
                        outputLockpick = args[++i];
                    break;

                case "--miscitem":
                    if (i + 1 < args.Length)
                        outputMiscItem = args[++i];
                    break;

                case "--npc":
                    if (i + 1 < args.Length)
                        outputNpc = args[++i];
                    break;

                case "--probe":
                    if (i + 1 < args.Length)
                        outputProbe = args[++i];
                    break;

                case "--race":
                    if (i + 1 < args.Length)
                        outputRace = args[++i];
                    break;

                case "--skill":
                    if (i + 1 < args.Length)
                        outputSkill = args[++i];
                    break;

                case "--spell":
                    if (i + 1 < args.Length)
                        outputSpell = args[++i];
                    break;

                case "--weapon":
                    if (i + 1 < args.Length)
                        outputWeapon = args[++i];
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
            _ => throw new ArgumentException($"Unknown output format: {outputFormat}")
        };

        List<Npc> npcs = new List<Npc>();
        List<Cell> cells = new List<Cell>();
        List<Expansion> expansions = new List<Expansion>(); //tracks which JSON file an NPC came from

        List<Dialogue> dialogues = new List<Dialogue>();
        List<DialogueInfo> dialogueInfos = new List<DialogueInfo>();
        List<Book> books = new List<Book>();
        List<MiscItem> miscItems = new List<MiscItem>();
        List<Clothing> clothes = new List<Clothing>();
        List<Enchanting> enchantings = new List<Enchanting>();
        List<Weapon> weapons = new List<Weapon>();
        List<Spell> spells = new List<Spell>();
        List<Armor> armors = new List<Armor>();
        List<Effect> effects = new List<Effect>();
        List<Alchemy> alchemies = new List<Alchemy>();
        List<Ingredient> ingredients = new List<Ingredient>();
        List<Creature> creatures = new List<Creature>();
        List<Birthsign> birthsigns = new List<Birthsign>();
        List<Race> races = new List<Race>();
        List<Apparatus> apparatuses = new List<Apparatus>();
        List<Class> classes = new List<Class>();
        List<Faction> factions = new List<Faction>();
        List<Skill> skills = new List<Skill>();
        List<Probe> probes = new List<Probe>();
        List<Lockpick> lockpicks = new List<Lockpick>();

        //used to populate the topic of DialogueInfo. DialogInfo related to a specific topic appear of Dialogue object
        string DialogueTopic = "";
        int DialogueId = 0;

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
            if(verbose) Console.WriteLine("Expansion name added: " + stripped);
        }
        /*Check for special npc.json file
         npc.json file can be generated if you need attributes & skills from npc's with autocalculate on.
        To generate an npc.json file first create an npc.esm file by opening the .esp/.esm files(s) in the construction
        set and toggleing autocalculate off for those npc. Save the result as npc.esm then run tes3conv to convert to
        npc.json. If found, npc.json by default will be read before any other json files, but will be excluded from
        Expansion column assignment */
        expansionFilePaths.AddRange(jsonFilePaths);
        bool hasNpcJson = (expansionNames.Contains("npc")) ? true : false;
        string npcJsonPath = string.Empty;
        if (hasNpcJson)
        {
            if(verbose) Console.WriteLine("npc.json file found.");
            npcJsonPath = Path.Combine(currentDir, "npc.json");
            jsonFilePaths.Remove(npcJsonPath);
            expansionFilePaths.Remove(npcJsonPath);
            expansionNames.Remove("npc"); // List<string> containing the extracted expansion file names
            jsonFilePaths.Insert(0, npcJsonPath);
        }
        else
        {
            if(verbose) Console.WriteLine("no npc.json file found.");
        }

        int expansionIndex = 0;

        if(verbose) Console.WriteLine("Creating NPC - Expansion Map...");

        foreach (string expansionFilePath in expansionFilePaths)
        {
            using FileStream expansionFs = File.OpenRead(expansionFilePath);
            using JsonDocument expansionDoc = JsonDocument.Parse(expansionFs);
            JsonElement expansionRoot = expansionDoc.RootElement;
            if (expansionRoot.ValueKind == JsonValueKind.Array)
            {
                if(verbose) Console.WriteLine($"Reading {expansionNames[expansionIndex]}:");
                foreach (JsonElement element in expansionRoot.EnumerateArray())
                {
                    if (element.TryGetProperty("type", out JsonElement type))
                    {
                        switch (type.GetString()) {

                            case "Npc":
                                Models.Expansion expansion = Functions.SetExpansion(expansionNames[expansionIndex], element);
                                if (!expansions.Any(p => p.NPCId == expansion.NPCId))
                                {
                                    expansions.Add(expansion);
                                }
                                else
                                {
                                    string alreadyAddedTo = expansions.Find(x => x.NPCId == expansion.NPCId)?.Name ?? "";
                                    if(verbose) Console.WriteLine(expansionNames[expansionIndex] + " " + expansion.NPCId + " already added to " + alreadyAddedTo);
                                }
                                break;

                            case "Dialogue":
                                var dialogue = Functions.DeserializeObject<Dialogue>(element);
                                dialogue.dialogue_id = DialogueId;
                                DialogueTopic = dialogue.id ?? "";
                                DialogueId++;
                                dialogues.Add(dialogue);
                                break;

                            case "DialogueInfo":
                                var dialogueInfo = Functions.DeserializeObject<DialogueInfo>(element);
                                if(!dialogueInfos.Any(d => d.id == dialogueInfo.id))
                                {
                                    dialogueInfo.expansion = expansionNames[expansionIndex];
                                    dialogueInfo.dialogue_topic = DialogueTopic;
                                    dialogueInfo.dialogue_id = DialogueId - 1;
                                    dialogueInfos.Add(dialogueInfo);
                                }
                                break;

                            case "Book":
                                var book = Functions.DeserializeObject<Book>(element);
                                if (!books.Any(b => b.id == book.id))
                                {
                                    book.expansion = expansionNames[expansionIndex];
                                    books.Add(book);
                                }
                                break;

                            case "MiscItem":
                                var miscItem = Functions.DeserializeObject<MiscItem>(element);
                                if(!miscItems.Any(m => m.id == miscItem.id))
                                {
                                    miscItem.expansion = expansionNames[expansionIndex];
                                    miscItems.Add(miscItem);
                                }
                                break;

                            case "Clothing":
                                var cloth = Functions.DeserializeObject<Clothing>(element);
                                if (!clothes.Any(c => c.id == cloth.id)) {
                                    cloth.expansion = expansionNames[expansionIndex];
                                    clothes.Add(cloth);
                                }
                                break;

                            case "Enchanting":
                                var enchant = Functions.DeserializeObject<Enchanting>(element);
                                if(!enchantings.Any(e => e.id == enchant.id))
                                {
                                    enchantings.Add(enchant);
                                }
                                else
                                {
                                    enchantings.RemoveAll(e => e.id == enchant.id);
                                    enchantings.Add(enchant);
                                }
                                break;
                            case "Weapon":
                                var weapon = Functions.DeserializeObject<Weapon>(element);
                                if (!weapons.Any(w => w.id == weapon.id))
                                {
                                    weapon.expansion = expansionNames[expansionIndex];
                                    weapons.Add(weapon);
                                }
                                break;
                            case "Spell":
                                var spell = Functions.DeserializeObject<Spell>(element);
                                if (!spells.Any(s => s.id == spell.id))
                                {
                                    spell.expansion = expansionNames[expansionIndex];
                                    spells.Add(spell);
                                }
                                break;
                            case "Armor":
                                var armor = Functions.DeserializeObject<Armor>(element);
                                if (!armors.Any(a => a.id == armor.id))
                                {
                                    armor.expansion = expansionNames[expansionIndex];
                                    armors.Add(armor);
                                }
                                break;
                            case "Alchemy":
                                var alchemy = Functions.DeserializeObject<Alchemy>(element);
                                if(!alchemies.Any(a => a.id == alchemy.id))
                                {
                                    alchemy.expansion = expansionNames[expansionIndex];
                                    alchemies.Add(alchemy);
                                }
                                break;
                            case "Ingredient":
                                var ingredient = Functions.DeserializeObject<Ingredient>(element);
                                if (!ingredients.Any(i => i.id == ingredient.id))
                                {
                                    ingredient.expansion = expansionNames[expansionIndex];
                                    ingredients.Add(ingredient);
                                }
                                break;

                            case "MagicEffect":
                                var effect = Functions.DeserializeObject<Effect>(element);
                                if (!effects.Any(e => e.id == effect.id))
                                {
                                    effect.expansion = expansionNames[expansionIndex];
                                    effects.Add(effect);
                                }
                                break;
                            case "Creature":
                                var creature = Functions.DeserializeObject<Creature>(element);
                                if (!creatures.Any(c => c.id == creature.id))
                                {
                                    creature.expansion = expansionNames[expansionIndex];
                                    creatures.Add(creature);
                                }
                                break;
                            case "Birthsign":
                                var birthsign = Functions.DeserializeObject<Birthsign>(element);
                                if (!birthsigns.Any(b => b.id == birthsign.id))
                                {
                                    birthsigns.Add(birthsign);
                                }
                                break;
                            case "Race":
                                var race = Functions.DeserializeObject<Race>(element);
                                if(!races.Any(r => r.id == race.id))
                                {
                                    race.expansion = expansionNames[expansionIndex];
                                    races.Add(race);
                                } 
                                break;
                            case "Apparatus":
                                var apparatus = Functions.DeserializeObject<Apparatus>(element);
                                if (!apparatuses.Any(a => a.id == apparatus.id))
                                {
                                    apparatus.expansion = expansionNames[expansionIndex];
                                    apparatuses.Add(apparatus);
                                }
                                break;
                            case "Class":
                                var className = Functions.DeserializeObject<Class>(element);
                                if (!classes.Any(c => c.id == className.id))
                                {
                                    className.expansion = expansionNames[expansionIndex];
                                    classes.Add(className);
                                }
                                break;
                            case "Faction":
                                var faction = Functions.DeserializeObject<Faction>(element);
                                if (!factions.Any(f => f.id == faction.id))
                                {
                                    faction.expansion = expansionNames[expansionIndex];
                                    factions.Add(faction);
                                }
                                break;
                            case "Skill":
                                var skill = Functions.DeserializeObject<Skill>(element);
                                if (!skills.Any(s => s.id == skill.id))
                                {
                                    skill.expansion = expansionNames[expansionIndex];
                                    skills.Add(skill);
                                }
                                break;
                            case "Lockpick":
                                var lockpick = Functions.DeserializeObject<Lockpick>(element);
                                if (!lockpicks.Any(l => l.id == lockpick.id))
                                {
                                    lockpick.expansion = expansionNames[expansionIndex];
                                    lockpicks.Add(lockpick);
                                }
                                break;
                            case "Probe":
                                var probe = Functions.DeserializeObject<Probe>(element);
                                if (!probes.Any(p => p.id == probe.id))
                                {
                                    probe.expansion = expansionNames[expansionIndex];
                                    probes.Add(probe);
                                }
                                break;
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("Expecting an array.");
            }
            if (verbose) {
                Console.WriteLine($"After Expansion {expansionNames[expansionIndex]}:");
                Console.WriteLine($"{expansions.Count} NPCs, {dialogues.Count} Dialogues, {books.Count} Books, {miscItems.Count} MiscItems, {clothes.Count} Clothing");
                Console.WriteLine($"{enchantings.Count} Enchantments, {weapons.Count} Weapons, {spells.Count} Spells, {armors.Count} Armors, {alchemies.Count} Alchemies");
                Console.WriteLine($"{ingredients.Count} Ingredients, {effects.Count} Effects, {creatures.Count} Creatures, {birthsigns.Count} Birthsigns, {races.Count} Races");
                Console.WriteLine($"{apparatuses.Count} Apparatuses, {classes.Count} Classes, {factions.Count} Factions, {skills.Count} Skills, {lockpicks.Count} Lockpicks");
                Console.WriteLine($"{probes.Count} Probes");
            } 
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
            if(verbose) Console.WriteLine($"After {Path.GetFileName(path)} : {npcs.Count} NPCs found, skipped {skipCount} already added, and {cells.Count} Cells ");
        }

        //populate location & expansion informatin of NPCs
        Console.WriteLine("Adding Cell & Region to NPCs (can take a few minutes)...");
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
                if (npc.CellName == null && npc.Region == null && verbose) Console.WriteLine("CellName & Region mssing - " + npc.Id);
                if (npc.Attributes == null && verbose) Console.WriteLine($"Attributes missing - {npc.Id}");
                if (npc.Skills == null && verbose) Console.WriteLine($"Skills missing - " + npc.Id);
                if(npc.Expansion == null && verbose) Console.WriteLine($"Expansion missing - " + npc.Id);
            }

            // Remove objects from list that we don't want to include
            npcs.RemoveAll(item => item.Attributes == null);
            npcs.RemoveAll(item => (item.CellName == null && item.Region == null));
        }
        //Remove template npcs
        npcs.RemoveAll(item => (item.Expansion == null));

        Console.WriteLine("Writing output files...");

        string outputFile = $"{prefix}{outputNpc}.{fileExtension}";
        string outputFileDialogue = $"{prefix}{outputDialogue}.{fileExtension}";
        string outputFileDialogueInfo = $"{prefix}{outputDialogueInfo}.{fileExtension}";
        string outputFileBook = $"{prefix}{outputBook}.{fileExtension}";
        string outputFileMiscItem = $"{prefix}{outputMiscItem}.{fileExtension}";
        string outputFileClothing = $"{prefix}{outputClothing}.{fileExtension}";
        string outputFileEnchanting = $"{prefix}{outputEnchanting}.{fileExtension}";
        string outputFileWeapon = $"{prefix}{outputWeapon}.{fileExtension}";
        string outputFileSpell = $"{prefix}{outputSpell}.{fileExtension}";
        string outputFileArmor = $"{prefix}{outputArmor}.{fileExtension}";
        string outputFileEffect = $"{prefix}{outputEffect}.{fileExtension}";
        string outputFileAlchemy = $"{prefix}{outputAlchemy}.{fileExtension}";
        string outputFileIngredient = $"{prefix}{outputIngredient}.{fileExtension}";
        string outputFileCreature = $"{prefix}{outputCreature}.{fileExtension}";
        string outputFileBirthsign = $"{prefix}{outputBirthsign}.{fileExtension}";
        string outputFileRace = $"{prefix}{outputRace}.{fileExtension}";
        string outputFileApparatus = $"{prefix}{outputApparatus}.{fileExtension}";
        string outputFileClass = $"{prefix}{outputClass}.{fileExtension}";
        string outputFileFaction = $"{prefix}{outputFaction}.{fileExtension}";
        string outputFileSkill = $"{prefix}{outputSkill}.{fileExtension}";
        string outputFileLockpick = $"{prefix}{outputLockpick}.{fileExtension}";
        string outputFileProbe = $"{prefix}{outputProbe}.{fileExtension}";

        string format = outputFormat.ToLowerInvariant();
        switch (format)
        {
            case "csv":
            case "tsv":
                FileWriter.WriteCsv(outputFile, npcs, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileDialogue, dialogues, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileDialogueInfo, dialogueInfos, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileBook, books, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileMiscItem, miscItems, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileClothing, clothes, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileEnchanting, enchantings, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileWeapon, weapons, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileSpell, spells, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileArmor, armors, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileEffect, effects, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileAlchemy, alchemies, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileIngredient, ingredients, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileCreature, creatures, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileBirthsign, birthsigns, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileRace, races, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileApparatus, apparatuses, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileClass, classes, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileFaction, factions, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileSkill, skills, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileLockpick, lockpicks, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileProbe, probes, includeColumnHeadings, format);
                break;
            case "mysql":
            case "postgres":
                FileWriter.WriteSql(outputFile, npcs, outputNpc, format);
                FileWriter.WriteSql(outputFileDialogue, dialogues, outputDialogue, format);
                FileWriter.WriteSql(outputFileDialogueInfo, dialogueInfos, outputDialogueInfo, format);
                FileWriter.WriteSql(outputFileBook, books, outputBook, format);
                FileWriter.WriteSql(outputFileMiscItem, miscItems, outputMiscItem, format);
                FileWriter.WriteSql(outputFileClothing, clothes, outputClothing, format);
                FileWriter.WriteSql(outputFileEnchanting, enchantings, outputEnchanting, format);
                FileWriter.WriteSql(outputFileWeapon, weapons, outputWeapon, format);
                FileWriter.WriteSql(outputFileSpell, spells, outputSpell, format);
                FileWriter.WriteSql(outputFileArmor, armors, outputArmor, format);
                FileWriter.WriteSql(outputFileEffect, effects, outputEffect, format);
                FileWriter.WriteSql(outputFileAlchemy, alchemies, outputAlchemy, format);
                FileWriter.WriteSql(outputFileIngredient, ingredients, outputIngredient, format);
                FileWriter.WriteSql(outputFileCreature, creatures, outputCreature, format);
                FileWriter.WriteSql(outputFileBirthsign, birthsigns, outputBirthsign, format);
                FileWriter.WriteSql(outputFileRace, races, outputRace, format);
                FileWriter.WriteSql(outputFileApparatus, apparatuses, outputApparatus, format);
                FileWriter.WriteSql(outputFileClass, classes, outputClass, format);
                FileWriter.WriteSql(outputFileFaction, factions, outputFaction, format);
                FileWriter.WriteSql(outputFileSkill, skills, outputSkill, format);
                FileWriter.WriteSql(outputFileLockpick, lockpicks, outputLockpick, format);
                FileWriter.WriteSql(outputFileProbe, probes, outputProbe, format);
                break;
            default:
                Console.WriteLine("Unsupported output file format.");
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
        Console.WriteLine("  --format, -f <type>      Output type: csv tsv mysql postgres (default: csv)");
        Console.WriteLine("  --no-headers             Exclude column headers in (csv/tsv only)");
        Console.WriteLine("  --skip                   Will skip NPCs missing Cell, Region, Attribute or Skill poperties");
        Console.WriteLine("  --verbose                Display verbose output");
        Console.WriteLine("");
        Console.WriteLine("  --prefix, -p <prefix>    Prefix for output files, helpful if keeping expansions separate (default: none)");
        Console.WriteLine("");
        Console.WriteLine("  --alchemy <name>         db Table & output File name for extracted Alchemies (default: alchemy)");
        Console.WriteLine("  --apparatus <name>       db Table & output File name for extracted Apparatuses (default: apparatus)");
        Console.WriteLine("  --armor <name>           db Table & output File name for extracted Armors (default: armor)");
        Console.WriteLine("  --birthsign <name>       db Table & output File name for extracted Birthsigns (default: birthsign)");
        Console.WriteLine("  --book <name>            db Table & output File name for extracted Books (default: book)");
        Console.WriteLine("  --class <name>           db Table & output File name for extracted Classes (default: class)");
        Console.WriteLine("  --clothing <name>        db Table & output File name for extracted Clothing (default: clothing)");
        Console.WriteLine("  --creature <name>        db Table & output File name for extracted Creatures (default: creature)");
        Console.WriteLine("  --dialogue <name>        db Table & output File name for extracted Dialogue (default: dialogue)");
        Console.WriteLine("  --dialogueinfo <name>    db Table & output File name for extracted DialogueInfo (default: dialogueinfo)");
        Console.WriteLine("  --effect <name>          db Table & output File name for extracted Effects (default: effect)");
        Console.WriteLine("  --enchanting <name>      db Table & output File name for extracted Enchantings (default: enchanting)");
        Console.WriteLine("  --faction <name>         db Table & output File name for extracted Factions (default: faction)");
        Console.WriteLine("  --ingredient <name>      db Table & output File name for extracted Ingredients (default: ingredient)");
        Console.WriteLine("  --lockpick <name>        db Table & output File name for extracted Lockpicks (default: lockpick)");
        Console.WriteLine("  --miscitem <name>        db Table & output File name for extracted MiscItems (default: miscitem)");
        Console.WriteLine("  --npc <name>             db Table & output File name for extracted NPCs (default: npc)");
        Console.WriteLine("  --probe <name>           db Table & output File name for extracted Probes (default: probe)");
        Console.WriteLine("  --race <name>            db Table & output File name for extracted Races (default: race)");
        Console.WriteLine("  --skill <name>           db Table & output File name for extracted Skills (default: skill)");
        Console.WriteLine("  --spell <name>           db Table & output File name for extracted Spells (default: spell)");
        Console.WriteLine("  --weapon <name>          db Table & output File name for extracted Weapons (default: weapon)");
        Console.WriteLine("");
        Console.WriteLine("  --help                   Show this help message");
    }
}//end of Program
