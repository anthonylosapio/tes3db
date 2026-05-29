namespace tes3db;
using System.Collections.Generic;

public class Models
{
    public class Npc
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Race { get; set; }
        public string? Class { get; set; }
        public string? Faction { get; set; }
        public string? Flags { get; set; }
        public string? Gender { get; set; }
        public bool? IsEssential { get; set; }
        public bool? IsPersistent { get; set; }
        public List<InventoryItem>? Inventory { get; set; }
        public List<string>? Spells { get; set; }
        public string? Location { get; set; }
        public string? SubLocation { get; set; }
        public string? CellName { get; set; }
        public string? Region { get; set; }
        public bool? IsInterior { get; set; }
        public string? Expansion { get; set; }
        public int? Level { get; set; }
        public int? Health { get; set; }
        public int? Magicka { get; set; }
        public int? Fatigue { get; set; }
        public int? Hello { get; set; }
        public int? Fight { get; set; }
        public int? Flee { get; set; }
        public int? Alarm { get; set; }
        public int? Disposition { get; set; }
        public int? Reputation { get; set; }
        public int? Rank { get; set; }
        public int? Gold { get; set; }
        public bool? No_Services { get; set; }
        public bool? BARTERS_WEAPONS { get; set; }
        public bool? BARTERS_ARMOR { get; set; }
        public bool? BARTERS_REPAIR_ITEMS { get; set; }
        public bool? BARTERS_INGREDIENTS { get; set; }
        public bool? BARTERS_ALCHEMY { get; set; }
        public bool? BARTERS_BOOKS { get; set; }
        public bool? BARTERS_CLOTHING { get; set; }
        public bool? BARTERS_LIGHTS { get; set; }
        public bool? BARTERS_MISC_ITEMS { get; set; }
        public bool? BARTERS_LOCKPICKS { get; set; }
        public bool? BARTERS_PROBES { get; set; }
        public bool? BARTERS_APPARATUS { get; set; }
        public bool? BARTERS_ENCHANTED_ITEMS { get; set; }
        public bool? OFFERS_SPELLMAKING { get; set; }
        public bool? OFFERS_SPELLS { get; set; }
        public bool? OFFERS_REPAIRS { get; set; }
        public bool? OFFERS_ENCHANTING { get; set; }
        public bool? OFFERS_TRAINING { get; set; }
        public bool? OFFERS_TRAVEL { get; set; }
        public Attributes Attributes { get; set; } = new Attributes();
        public Skills Skills { get; set; } = new Skills();

    }


    public class InventoryItem
    {
        public int? Quantity { get; set; }
        public string? ItemId { get; set; }
    }
    public class Attributes
    {
        public int? Strength { get; set; }
        public int? Intelligence { get; set; }
        public int? Willpower { get; set; }
        public int? Agility { get; set; }
        public int? Speed { get; set; }
        public int? Endurance { get; set; }
        public int? Personality { get; set; }
        public int? Luck { get; set; }
    }
    public class Skills
    {
        public int? Acrobatics { get; set; } = 0; // 20
        public int? Alchemy { get; set; } = 0; // 16
        public int? Alteration { get; set; } = 0;// 11
        public int? Armorer { get; set; } = 0; // 1
        public int? Athletics { get; set; } = 0; // 8
        public int? Axe { get; set; } = 0; // 6
        public int? Block { get; set; } // 0
        public int? BluntWeapon { get; set; } // 4
        public int? Conjuration { get; set; } // 13
        public int? Destruction { get; set; } // 10
        public int? Enchant { get; set; } // 9
        public int? HandToHand { get; set; } // 26
        public int? HeavyArmor { get; set; } // 3
        public int? Illusion { get; set; } // 12
        public int? LightArmor { get; set; } // 21
        public int? LongBlade { get; set; } // 5
        public int? Marksman { get; set; } // 23
        public int? MediumArmor { get; set; } // 2
        public int? Mercantile { get; set; } // 24
        public int? Mysticism { get; set; } // 14
        public int? Restoration { get; set; } // 15
        public int? Security { get; set; } // 18
        public int? ShortBlade { get; set; } // 22
        public int? Sneak { get; set; } // 19
        public int? Spear { get; set; } // 7
        public int? Speechcraft { get; set; } // 25
        public int? Unarmored { get; set; } // 17
    }
    public class Expansion
    {
        public string? Name { get; set; }
        public string? NPCId { get; set; }
    }
    public class Cell
    {
        public string? CellName { get; set; }
        public bool? IsInterior { get; set; }
        public string? CellRegion { get; set; }
        public string? Location { get; set; }
        public string? SubLocation { get; set; }
        public List<string>? CellRefs { get; set; }
    }

    public class FieldValueandType { 
    
        public object? Value { get; set; }
        public Type? Type { get; set; }
    }

    public class DialogueInfo { 
        public string? id { get; set; }
        public string? prev_id { get; set; }
        public string? next_id { get; set; }
        public string? speaker_id { get; set; }
        public string? text { get; set; }
    }
}