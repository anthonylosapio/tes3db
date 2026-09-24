namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static tes3db.Models;

public class FileReader
{
    public static void Main(bool verbose, bool noSkip, bool includeColumnHeadings, string prefix, string fileExtension, string outputDirectory, string outputFormat)
    {
        // filename/table values
        string outputAlchemy = "alchemy";
        string outputApparatus = "apparatus";
        string outputArmor = "armor";
        string outputBirthsign = "birthsign";
        string outputBook = "book";
        string outputCell = "cell";
        string outputClass = "class";
        string outputClothing = "clothing";
        string outputCreature = "creature";
        string outputDialogueInfo = "dialogueinfo";
        string outputMagicEffect = "magiceffect";
        string outputEnchanting = "enchanting";
        string outputFaction = "faction";
        string outputIngredient = "ingredient";
        string outputLockpick = "lockpick";
        string outputMiscItem = "miscitem";
        string outputNpc = "npc";
        string outputProbe = "probe";
        string outputRace = "race";
        string outputRepairItem = "repairitem";
        string outputSkill = "skill";
        string outputSpell = "spell";
        string outputWeapon = "weapon";
        string outputHeader = "header";

        List<Npc> npcs = new List<Npc>();
        List<Cell> cells = new List<Cell>();
        List<Dialogue> dialogues = new List<Dialogue>();
        List<DialogueInfo> dialogueInfos = new List<DialogueInfo>();
        List<Book> books = new List<Book>();
        List<MiscItem> miscItems = new List<MiscItem>();
        List<Clothing> clothes = new List<Clothing>();
        List<Enchanting> enchantings = new List<Enchanting>();
        List<Weapon> weapons = new List<Weapon>();
        List<Spell> spells = new List<Spell>();
        List<Armor> armors = new List<Armor>();
        List<MagicEffect> effects = new List<MagicEffect>();
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
        List<Header> headers = new List<Header>();
        List<RepairItem> repairItems = new List<RepairItem>();

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
            if (verbose) Console.WriteLine("Expansion name added: " + stripped);
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
            if (verbose) Console.WriteLine("npc.json file found.");
            npcJsonPath = Path.Combine(currentDir, "npc.json");
            jsonFilePaths.Remove(npcJsonPath);
            expansionFilePaths.Remove(npcJsonPath);
            expansionNames.Remove("npc"); // List<string> containing the extracted expansion file names
            jsonFilePaths.Insert(0, npcJsonPath); //move the npc.json file to the front of the list
        }
        else
        {
            if (verbose) Console.WriteLine("no npc.json file found.");
        }

        int expansionIndex = 0;

        if (verbose) Console.WriteLine("Reading npc.json...");
        using FileStream npcFs = File.OpenRead(npcJsonPath);
        using JsonDocument npcDoc = JsonDocument.Parse(npcFs);
        JsonElement npcRoot = npcDoc.RootElement;
        if (npcRoot.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement element in npcRoot.EnumerateArray())
            {
                if (element.TryGetProperty("type", out JsonElement type))
                {
                    if (type.GetString() == "Npc")
                    {
                        var npc = Functions.DeserializeObject<Npc>(element);
                        if (!npcs.Any(n => n.id == npc.id))
                        {
                            npcs.Add(npc);
                            Console.Write($"\r {npc.id}");
                        }
                    }
                }
            }
        }
        if (verbose) Console.WriteLine($"Found {npcs.Count} NPCs in npc.json.");

        foreach (string expansionFilePath in expansionFilePaths)
        {
            using FileStream expansionFs = File.OpenRead(expansionFilePath);
            using JsonDocument expansionDoc = JsonDocument.Parse(expansionFs);
            JsonElement expansionRoot = expansionDoc.RootElement;
            if (expansionRoot.ValueKind == JsonValueKind.Array)
            {
                if (verbose) Console.WriteLine($"Reading {expansionNames[expansionIndex]}:");
                foreach (JsonElement element in expansionRoot.EnumerateArray())
                {
                    if (element.TryGetProperty("type", out JsonElement type))
                    {
                        switch (type.GetString())
                        {

                            case "Cell":
                                var cell = Functions.DeserializeObject<Cell>(element);
                                if (!cells.Any(c => c.id == cell.id))
                                {
                                    cell.expansion = expansionNames[expansionIndex];
                                    cells.Add(cell);
                                }
                                break;
                            case "Npc":
                                var npc = Functions.DeserializeObject<Npc>(element);
                                if (!npcs.Any(n => n.id == npc.id))
                                {
                                    npc.expansion = expansionNames[expansionIndex];
                                    npcs.Add(npc);
                                }
                                else
                                {
                                    var existingNpc = npcs.FirstOrDefault(n => n.id == npc.id);
                                    //if the npc was added from npc.json, it will not have an expansion assigned. Assign it now.
                                    if (existingNpc.expansion == null) existingNpc.expansion = expansionNames[expansionIndex];
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
                                if (!dialogueInfos.Any(d => d.id == dialogueInfo.id))
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
                                if (!miscItems.Any(m => m.id == miscItem.id))
                                {
                                    miscItem.expansion = expansionNames[expansionIndex];
                                    miscItems.Add(miscItem);
                                }
                                break;

                            case "Clothing":
                                var cloth = Functions.DeserializeObject<Clothing>(element);
                                if (!clothes.Any(c => c.id == cloth.id))
                                {
                                    cloth.expansion = expansionNames[expansionIndex];
                                    clothes.Add(cloth);
                                }
                                break;

                            case "Enchanting":
                                var enchant = Functions.DeserializeObject<Enchanting>(element);
                                if (!enchantings.Any(e => e.id == enchant.id))
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
                                if (!alchemies.Any(a => a.id == alchemy.id))
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
                                var effect = Functions.DeserializeObject<MagicEffect>(element);
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
                                if (!races.Any(r => r.id == race.id))
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
                                    //this is here specifically because buoyant armiger has a blank name in the base game file
                                    if (string.IsNullOrEmpty(className.name)) className.name = className.id;
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
                            case "RepairItem":
                                var repairItem = Functions.DeserializeObject<RepairItem>(element);
                                if (!repairItems.Any(r => r.id == repairItem.id))
                                {
                                    repairItem.expansion = expansionNames[expansionIndex];
                                    repairItems.Add(repairItem);
                                }
                                break;
                            case "Header":
                                var header = Functions.DeserializeObject<Header>(element);
                                header.expansion = expansionNames[expansionIndex];
                                headers.Add(header);
                                break;
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("Expecting an array.");
            }
            if (verbose)
            {
                Console.WriteLine($"After Expansion {expansionNames[expansionIndex]}:");
                Console.WriteLine($" NPCs:         {npcs.Count,10} | Cells:        {cells.Count,10} | Weapons:      {weapons.Count,10}");
                Console.WriteLine($" Alchemies:    {alchemies.Count,10} | Apparatuses:  {apparatuses.Count,10} | Armors:       {armors.Count,10}");
                Console.WriteLine($" Birthsigns:   {birthsigns.Count,10} | Books:        {books.Count,10} | Classes:      {classes.Count,10}");
                Console.WriteLine($" Clothing:     {clothes.Count,10} | Creatures:    {creatures.Count,10} | Enchantments: {enchantings.Count,10}");
                Console.WriteLine($" Factions:     {factions.Count,10} | Ingredients:  {ingredients.Count,10} | Lockpicks:    {lockpicks.Count,10}");
                Console.WriteLine($" MagicEffects: {effects.Count,10} | MiscItems:    {miscItems.Count,10} | Probes:       {probes.Count,10}");
                Console.WriteLine($" Races:        {races.Count,10} | RepairItems:  {repairItems.Count,10} | Skills:       {skills.Count,10}");
                Console.WriteLine($" Spells:       {spells.Count,10} |");
            }
            expansionIndex++;
        }

        //Remove orphaned npcs
        npcs.RemoveAll(item => (item.expansion == null));

        //populate location, expansion, race, class & faction information for NPCs
        int npcTotal = npcs.Count;
        int npcCount = 0;
        if (verbose) Console.WriteLine("Adding race info to NPCs...");
        foreach (var npc in npcs)
        {
            Functions.AddRaceInfoToNPC(npc, races);
            npcCount++;
            if (verbose) Console.Write($"\r {npcCount}/{npcTotal}");
        }

        npcCount = 0;
        if (verbose) Console.WriteLine("");
        if (verbose) Console.WriteLine("Adding Class info to NPCs...");
        foreach (var npc in npcs)
        {
            Functions.AddClassInfoToNPC(npc, classes);
            npcCount++;
            if (verbose) Console.Write($"\r {npcCount}/{npcTotal}");
        }

        npcCount = 0;
        if (verbose) Console.WriteLine("");
        if (verbose) Console.WriteLine("Adding Faction info to NPCs...");
        foreach (var npc in npcs)
        {
            Functions.AddFactionInfoToNPC(npc, factions);
            npcCount++;
            if (verbose) Console.Write($"\r {npcCount}/{npcTotal}");
        }

        npcCount = 0;
        if (verbose) Console.WriteLine("");
        if (verbose) Console.WriteLine("Adding Cell, Region, & Location to NPCs...");

        var cellRefExpansionDictionary = new Dictionary<string, Dictionary<string, Cell>>();
        foreach (var expansion in expansionNames)
        {
            var index = Functions.BuildReferenceIndex(cells, expansion);
            cellRefExpansionDictionary[expansion] = index;
        }
        foreach (var npc in npcs)
        {
            Functions.AddCellLocationInfoToNPC(npc, cellRefExpansionDictionary[npc.expansion]);
            npcCount++;
            if (verbose) Console.Write($"\r {npc.expansion}: {npcCount}/{npcTotal}");
        }

        //adding a "None" faction to the faction list
        factions.Add(new Faction { id = "None", name = "None" });

        // List NPCs missing attributes, skills or cell placement
        if (!noSkip)
        {
            foreach (var npc in npcs)
            {
                if (npc.location == "None" && verbose) Console.WriteLine("CellName & Region mssing - " + npc.id);
                if (npc.data.stats.attributes == null && verbose) Console.WriteLine($"Attributes missing - {npc.id}");
                if (npc.data.stats.skills == null && verbose) Console.WriteLine($"Skills missing - " + npc.id);
                if (npc.expansion == null && verbose) Console.WriteLine($"Expansion missing - " + npc.id);
            }

            // Remove objects from list that we don't want to include
            npcs.RemoveAll(item => item.data.stats.attributes == null);
            npcs.RemoveAll(item => (item.location == "None"));
        }

        Console.WriteLine("Writing output files...");

        string outputFile = Path.Combine(outputDirectory, $"{prefix}{outputNpc}.{fileExtension}");
        //string outputFileDialogue = $"{prefix}{outputDialogue}.{fileExtension}";
        string outputFileDialogueInfo = Path.Combine(outputDirectory, $"{prefix}{outputDialogueInfo}.{fileExtension}");
        string outputFileBook = Path.Combine(outputDirectory, $"{prefix}{outputBook}.{fileExtension}");
        string outputFileMiscItem = Path.Combine(outputDirectory, $"{prefix}{outputMiscItem}.{fileExtension}");
        string outputFileCell = Path.Combine(outputDirectory, $"{prefix}{outputCell}.{fileExtension}");
        string outputFileClothing = Path.Combine(outputDirectory, $"{prefix}{outputClothing}.{fileExtension}");
        string outputFileEnchanting = Path.Combine(outputDirectory, $"{prefix}{outputEnchanting}.{fileExtension}");
        string outputFileWeapon = Path.Combine(outputDirectory, $"{prefix}{outputWeapon}.{fileExtension}");
        string outputFileSpell = Path.Combine(outputDirectory, $"{prefix}{outputSpell}.{fileExtension}");
        string outputFileArmor = Path.Combine(outputDirectory, $"{prefix}{outputArmor}.{fileExtension}");
        string outputFileMagicEffect = Path.Combine(outputDirectory, $"{prefix}{outputMagicEffect}.{fileExtension}");
        string outputFileAlchemy = Path.Combine(outputDirectory, $"{prefix}{outputAlchemy}.{fileExtension}");
        string outputFileIngredient = Path.Combine(outputDirectory, $"{prefix}{outputIngredient}.{fileExtension}");
        string outputFileCreature = Path.Combine(outputDirectory, $"{prefix}{outputCreature}.{fileExtension}");
        string outputFileBirthsign = Path.Combine(outputDirectory, $"{prefix}{outputBirthsign}.{fileExtension}");
        string outputFileRace = Path.Combine(outputDirectory, $"{prefix}{outputRace}.{fileExtension}");
        string outputFileApparatus = Path.Combine(outputDirectory, $"{prefix}{outputApparatus}.{fileExtension}");
        string outputFileClass = Path.Combine(outputDirectory, $"{prefix}{outputClass}.{fileExtension}");
        string outputFileFaction = Path.Combine(outputDirectory, $"{prefix}{outputFaction}.{fileExtension}");
        string outputFileSkill = Path.Combine(outputDirectory, $"{prefix}{outputSkill}.{fileExtension}");
        string outputFileLockpick = Path.Combine(outputDirectory, $"{prefix}{outputLockpick}.{fileExtension}");
        string outputFileProbe = Path.Combine(outputDirectory, $"{prefix}{outputProbe}.{fileExtension}");
        string outputFileHeader = Path.Combine(outputDirectory, $"{prefix}{outputHeader}.{fileExtension}");
        string outputFileRepairItem = Path.Combine(outputDirectory, $"{prefix}{outputRepairItem}.{fileExtension}");

        object[] listsObject = { npcs, /*dialogues,*/ dialogueInfos, books, miscItems, cells, clothes, enchantings, weapons, spells, armors, effects, alchemies, ingredients, creatures, birthsigns, races, apparatuses, classes, factions, skills, lockpicks, probes, headers, repairItems };
        string[] tableNames = { outputNpc,/* outputDialogue, */outputDialogueInfo, outputBook, outputMiscItem, outputCell, outputClothing, outputEnchanting, outputWeapon, outputSpell, outputArmor, outputMagicEffect, outputAlchemy, outputIngredient, outputCreature, outputBirthsign, outputRace, outputApparatus, outputClass, outputFaction, outputSkill, outputLockpick, outputProbe, outputHeader, outputRepairItem };

        string format = outputFormat.ToLowerInvariant();
        switch (format)
        {
            case "csv":
            case "tsv":
                FileWriter.WriteCsv(outputFile, npcs, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileDialogueInfo, dialogueInfos, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileBook, books, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileMiscItem, miscItems, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileClothing, clothes, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileEnchanting, enchantings, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileWeapon, weapons, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileSpell, spells, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileArmor, armors, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileMagicEffect, effects, includeColumnHeadings, format);
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
                FileWriter.WriteCsv(outputFileHeader, headers, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileRepairItem, repairItems, includeColumnHeadings, format);
                FileWriter.WriteCsv(outputFileCell, cells, includeColumnHeadings, format);
                break;
            case "mysql":
            case "postgres":
            case "sqlite":
                FileWriter.WriteSql(outputFile, npcs, outputNpc, format);
                FileWriter.WriteSql(outputFileDialogueInfo, dialogueInfos, outputDialogueInfo, format);
                FileWriter.WriteSql(outputFileBook, books, outputBook, format);
                FileWriter.WriteSql(outputFileMiscItem, miscItems, outputMiscItem, format);
                FileWriter.WriteSql(outputFileClothing, clothes, outputClothing, format);
                FileWriter.WriteSql(outputFileEnchanting, enchantings, outputEnchanting, format);
                FileWriter.WriteSql(outputFileWeapon, weapons, outputWeapon, format);
                FileWriter.WriteSql(outputFileSpell, spells, outputSpell, format);
                FileWriter.WriteSql(outputFileArmor, armors, outputArmor, format);
                FileWriter.WriteSql(outputFileMagicEffect, effects, outputMagicEffect, format);
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
                FileWriter.WriteSql(outputFileHeader, headers, outputHeader, format);
                FileWriter.WriteSql(outputFileRepairItem, repairItems, outputRepairItem, format);
                FileWriter.WriteSql(outputFileCell, cells, outputCell, format);

                FileWriter.WriteSqlCreateTableFile(Path.Combine(outputDirectory, "tes3db.sql"), listsObject, tableNames, format, "tes3db");

                break;
            default:
                Console.WriteLine("Unsupported output file format.");
                break;
        }
    }

}

