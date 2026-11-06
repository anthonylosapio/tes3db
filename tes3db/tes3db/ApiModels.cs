using System.Globalization;
using System.Text.RegularExpressions;
using static tes3db.Models;

namespace tes3db;

public record SearchObject(string id, string name, string type);

public record HeaderObject(string expansion, string author, string description);

public record AlchemyObject(
    string flags,
    string id, 
    string name, 
    string mesh, 
    string icon, 
    string expansion,
    string effects,
    double weight,
    int value,
    string data_flags);

public class InterfaceObject {

    public string[] GroupBy { get; set; } = ConstantsObject.GROUPBY;
    public string[] Agg { get; set; } = ConstantsObject.AGG_FUNCTION;
    public string[] Objects { get; set; } = ConstantsObject.SEARCH_OBJECTS;
    public FiltersObject Filters { get; set; } = new FiltersObject();
    public SortsObject Sorts { get; set; } = new SortsObject();

    public class FiltersObject
    {
        public IReadOnlyList<string>? Race { get; set; }
        public IReadOnlyList<string>? Class { get; set; }
        public IReadOnlyList<string>? Faction { get; set; }
        public string[] Gender { get; set; } = ConstantsObject.GENDERS;
        public IReadOnlyList<string>? Expansion { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyList<string>>? Location { get; set; }
        public string[] Services { get; set; } = ConstantsObject.SERVICES;
        public string[] ClothingType { get; set; } = ConstantsObject.CLOTHING_TYPE;
        public string[] ArmorType { get; set; } = ConstantsObject.ARMOR_TYPE;
        public string[] WeaponType { get; set; } = ConstantsObject.WEAPON_TYPE;
        public string[] SpellType { get; set; } = ConstantsObject.SPELL_TYPE;
        public string[] CreatureType { get; set; } = ConstantsObject.CREATURE_TYPE;
        public string[] ApparatusType { get; set; } = ConstantsObject.APPARATUS_TYPE;
        public IReadOnlyList<string>? MagicEffect { get; set; }
        public string[] MagicSchool { get; set; } = ConstantsObject.MAGIC_SCHOOL;
        public string[] Skills { get; set; } = ConstantsObject.SKILLS;
        public string[] Attributes { get; set; } = ConstantsObject.ATTRIBUTES;
        public string[] Specialization { get; set; } = ConstantsObject.SPECIALIZATION;
        public string[] IsPlayable { get; set; } = ConstantsObject.PLAYABLE_FLAG;
        public string[] IsBeastRace { get; set; } = ConstantsObject.BEAST_RACE_FLAG;
    }
    public class SortsObject
    {
        public string[] SortBy { get; set; } = ConstantsObject.SORTBY;
        public string[] ItemSort { get; set; } = ConstantsObject.ITEM_SORTBY;
        public string[] ArmorSort { get; set; } = ConstantsObject.ARMOR_SORTBY;
        public string[] WeaponSort { get; set; } = ConstantsObject.WEAPON_SORTBY;
        public string[] SpellSort { get; set; } = ConstantsObject.SPELL_SORTBY;
        public string[] CreatureSort { get; set; } = ConstantsObject.CREATURE_SORTBY;
        public string[] MiscItemSort { get; set; } = ConstantsObject.MISCITEM_SORTBY;
        public string[] ApparatusSort { get; set; } = ConstantsObject.APPARATUS_SORTBY;
        public string[] ToolSort { get; set; } = ConstantsObject.TOOL_SORTBY;
        public string[] MagicEffectSort { get; set; } = ConstantsObject.MAGICEFFECT_SORTBY;
        public string[] ClassSort { get; set; } = ConstantsObject.CLASS_SORTBY;
        public string[] FactionSort { get; set; } = ConstantsObject.FACTION_SORTBY;
        public string[] RaceSort { get; set; } = ConstantsObject.RACE_SORTBY;
    }
    /*
            $header = $this->repo->RunSelectQuery(header_query);
            $count = $this->repo->RunSelectQuery(query_count_query);;

            $filters = [
                "Race" => RACES,
                "Class" => CLASSES,
                "Faction" => FACTIONS,
                "Gender" => GENDERS,
                "Expansion" => EXPANSIONS,
                "Location" => LOCATIONS,
                "Services" => SERVICES,
                "Clothing Type" => CLOTHING_TYPE,
                "Armor Type" => ARMOR_TYPE,
                "Weapon Type" => WEAPON_TYPE,
                "Spell Type" => SPELL_TYPE,
                "Creature Type" => CREATURE_TYPE,
                "Apparatus Type" => APPARATUS_TYPE,
                "Magic Effect" => MAGIC_EFFECT,
                "Magic School" => MAGIC_SCHOOL,
                "Skills" => SKILLS,
                "Attributes" => ATTRIBUTES,
                "Specialization" => SPECIALIZATION,
                "Is Playable" => PLAYABLE_FLAG,
                "Is Beast Race" => BEAST_RACE_FLAG,
            ];

            $data = [
                "Header" => $header,
                "Count" => $count[0],
                "Filters" => $filters,
                "Group By" => GROUPBY,
                "Agg" => AGG_FUNCTION,
                "Objects" => SEARCH_OBJECTS,
                "Sorts" => [
                    "Sort By" => SORTBY,
                    "Item Sort" => ITEM_SORTBY,
                    "Armor Sort" => ARMOR_SORTBY,
                    "Weapon Sort" => WEAPON_SORTBY,
                    "Spell Sort" => SPELL_SORTBY,
                    "Creature Sort" => CREATURE_SORTBY,
                    "MiscItem Sort" => MISCITEM_SORTBY,
                    "Apparatus Sort" => APPARATUS_SORTBY,
                    "Tool Sort" => TOOL_SORTBY,
                    "MegicEffect Sort" => MAGICEFFECT_SORTBY,
                    "Class Sort" => CLASS_SORTBY,
                    "Faction Sort" => FACTION_SORTBY,
                    "Race Sort" => RACE_SORTBY,
                ],
            ];
     */
}

public class ConstantsObject
{
    public static string[] GENDERS = ["FEMALE", "MALE"];

    public static string[] SERVICES = [
        "BARTERS_WEAPONS",
        "BARTERS_ARMOR",
        "BARTERS_REPAIR_ITEMS",
        "BARTERS_INGREDIENTS",
        "BARTERS_ALCHEMY",
        "BARTERS_BOOKS",
        "BARTERS_CLOTHING",
        "BARTERS_LIGHTS",
        "BARTERS_MISC_ITEMS",
        "BARTERS_LOCKPICKS",
        "BARTERS_PROBES",
        "BARTERS_APPARATUS",
        "BARTERS_ENCHANTED_ITEMS",
        "OFFERS_SPELLMAKING",
        "OFFERS_SPELLS",
        "OFFERS_REPAIRS",
        "OFFERS_ENCHANTING",
        "OFFERS_TRAINING",
        "OFFERS_TRAVEL"
        ];

    //DROP DOWN MENU CONSTANTS
    public static string[] GROUPBY = ["class", "faction", "race", "gender", "expansion", "location"];

    public static string[] AGG_FUNCTION = ["AVG", "SUM", "MIN", "MAX", "COUNT"];

    public static string[] LIMITS = ["10", "25", "50", "100", "200", "500"];

    public static string[] SORTBY = ["level",
        "gold",
        "health",
        "magicka",
        "fatigue",
        "hello",
        "fight",
        "flee",
        "alarm",
        "disposition",
        "reputation",
        "rank",
        "strength",
        "intelligence",
        "willpower",
        "agility",
        "speed",
        "endurance",
        "personality",
        "luck",
        "acrobatics",
        "alchemy",
        "alteration",
        "armorer",
        "athletics",
        "axe",
        "block",
        "bluntweapon",
        "conjuration",
        "destruction",
        "Enchant",
        "handtohand",
        "heavyarmor",
        "illusion",
        "lightarmor",
        "longblade",
        "marksman",
        "mediumarmor",
        "mercantile",
        "mysticism",
        "restoration",
        "security",
        "shortBlade",
        "sneak",
        "spear",
        "speechcraft",
        "unarmored"];

    public static string[] MAGIC_SCHOOL = ["Mysticism", "Illusion", "Conjuration", "Alteration", "Destruction", "Restoration"];

    public static string[] SKILLS = [
        "Acrobatics", 
        "Alchemy", 
        "Alteration", 
        "Armorer", 
        "Athletics", 
        "Axe", 
        "Block", 
        "BluntWeapon", 
        "Conjuration", 
        "Destruction", 
        "Enchant", 
        "HandToHand", 
        "HeavyArmor", 
        "Illusion", 
        "LightArmor", 
        "LongBlade", 
        "Marksman", 
        "MediumArmor", 
        "Mercantile", 
        "Mysticism", 
        "Restoration", 
        "Security", 
        "ShortBlade", 
        "Sneak", 
        "Spear", 
        "Speechcraft", 
        "Unarmored"];

    public static string[] ATTRIBUTES = ["Strength", "Intelligence", "Willpower", "Speed", "Agility", "Endurance", "Personality"];

    public static string[] SPECIALIZATION = ["Stealth", "Magic", "Combat"];

    public static string[] PLAYABLE_FLAG = ["Playable", "Non-Playable"];

    public static string[] BEAST_RACE_FLAG = ["Beast Race", "Non-Beast Race"];

    //SEARCH_OBJECTS populates the values in the Top X Search Dropdown
    public static string[] SEARCH_OBJECTS = [
        "npcs", 
        "alchemy", 
        "apparatuses", 
        "armors", 
        "books", 
        "classes",
        "clothing", 
        "creatures", 
        "factions", 
        "ingredients", 
        "lockpicks", 
        "magiceffects", 
        "miscitems", 
        "probes", 
        "races", 
        "repairitems", 
        "spells", 
        "weapons"];

    //_SORTBY is used to populate Top X Search Sort By drop down
    //_TYPE is used to create object specific checkbox filters
    /*ITEM_SORTBY used by books, clothing*/
    public static string[] ITEM_SORTBY = ["weight", "value", "enchantment"];
    /*TOOL_SORTBY used by lockpicks, probes, & repairitems */
    public static string[] TOOL_SORTBY = ["weight", "value", "quality", "uses"];

    public static string[] ARMOR_TYPE = [
        "Boots", 
        "LeftBracer", 
        "RightBracer", 
        "Cuirass", 
        "Greaves", 
        "Helmet", 
        "LeftPauldron", 
        "RightPauldron", 
        "Shield", 
        "LeftGauntlet", 
        "RightGauntlet"];

    public static string[] ARMOR_SORTBY = ["weight", "value", "health", "enchantment", "armor_rating"];

    public static string[] CLOTHING_TYPE = [
        "Ring", 
        "Robe", 
        "Amulet", 
        "Belt", 
        "LeftGlove", 
        "RightGlove", 
        "Pants", 
        "Shirt", 
        "Shoes", 
        "Skirt"];

    public static string[] CREATURE_TYPE = ["Normal", "Undead", "Humanoid", "Daedra"];

    public static string[] CREATURE_SORTBY = [
        "Hello", 
        "Fight",
        "Flee",
        "Alarm",
        "level",
        "strength",
        "intelligence",
        "willpower",
        "agility",
        "speed",
        "endurance",
        "personality",
        "luck",
        "health",
        "magicka",
        "fatigue",
        "soul",
        "combat",
        "magic",
        "stealth",
        "gold"];

    //ALSO USED BY ALCHEMY & INGREDIENTS
    public static string[] MISCITEM_SORTBY = ["weight", "value"];

    public static string[] SPELL_TYPE = ["Spell", "Power", "Ability", "Blight", "Disease", "Curse"];

    public static string[] SPELL_SORTBY = ["cost"];

    public static string[] WEAPON_TYPE = [
        "Arrow", 
        "AxeOneHand", 
        "AxeTwoHand", 
        "BluntOneHand", 
        "BluntTwoClose", 
        "BluntTwoWide", 
        "Bolt", 
        "LongBladeOneHand", 
        "LongBladeTwoClose", 
        "MarksmanBow", 
        "MarksmanCrossbow", 
        "MarksmanThrown", 
        "ShortBladeOneHand", 
        "SpearTwoWide"];

    public static string[] WEAPON_SORTBY = [
        "weight", 
        "value", 
        "health", 
        "speed", 
        "reach", 
        "enchantment", 
        "chop_min", 
        "chop_max", 
        "slash_min", 
        "slash_max", 
        "thrust_min", 
        "thrust_max"];

    public static string[] APPARATUS_SORTBY = ["weight", "value", "quality"];

    public static string[] APPARATUS_TYPE = ["Alembic", "Calcinator", "MortarAndPestle", "Retort"];

    public static string[] MAGICEFFECT_SORTBY = ["base_cost", "speed", "size", "size_cap"];

    public static string[] CLASS_SORTBY = [
        "specialization", 
        "attribute1", 
        "attribute2", 
        "major1", 
        "major2", 
        "major3",
        "major4",
        "major5", 
        "minor1",
        "minor2", 
        "minor3", 
        "minor4",
        "minor5"];

    public static string[] FACTION_SORTBY = ["favored_attribute1", "favored_attribute2", "expansion"];

    public static string[] RACE_SORTBY = [
        "female_height", 
        "female_weight",
        "female_agility", 
        "female_endurance",  
        "female_intelligence", 
        "female_luck", 
        "female_personality", 
        "female_speed", 
        "female_strength", 
        "female_willpower",
        "male_height", 
        "male_weight",
        "male_agility", 
        "male_endurance",  
        "male_intelligence", 
        "male_luck", 
        "male_personality", 
        "male_speed", 
        "male_strength", 
        "male_willpower"
    ];

}