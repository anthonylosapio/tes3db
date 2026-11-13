using SQLitePCL;
using static tes3db.Models;

namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class SqlQueries
{
	/// <summary>
	/// Used to populate the search trie with id, name, and type of all searchable objects
	/// </summary>
	public const string search_object_query = @"SELECT `id`, `name`, 'npc' AS `type` FROM `npc`	
		UNION ALL	
		SELECT `id`, `name`, 'book' AS `type` FROM `book` 
		WHERE 
			`name`!='' 
			AND `icon` NOT IN ('','default icon.dds') 
			AND `mesh` !='tr	r_editormarker_npc.nif'
			AND `text` != ''
		UNION ALL
		SELECT DISTINCT `dialogue_topic` AS 'id', `dialogue_topic` AS 'name', 'topic' AS 'type' 
		FROM `dialogueinfo` 
		WHERE 
			`dialogue_type` = 'Topic'
			AND `dialogue_topic`!=''
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'clothing' AS `type` FROM `clothing`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'birthsign' AS `type` FROM `birthsign`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'armor' AS `type` FROM `armor`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'spell' AS `type` FROM `spell`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'weapon' AS `type` FROM `weapon`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'creature' AS `type` FROM `creature`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'miscitem' AS `type` FROM `miscitem`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'apparatus' AS `type` FROM `apparatus`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'probe' AS `type` FROM `probe`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'lockpick' AS `type` FROM `lockpick`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'repairitem' AS `type` FROM `repairitem`
		UNION ALL
		SELECT DISTINCT `id`, `id` AS 'name', 'magiceffect' AS `type` FROM `magiceffect`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'alchemy' AS `type` FROM `alchemy`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'ingredient' AS `type` FROM `ingredient`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'class' AS `type` FROM `class`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'faction' AS `type` FROM `faction`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'race' AS `type` FROM `race`
		UNION ALL
		SELECT DISTINCT `id`, `id` AS 'name', 'skill' AS `type` FROM `skill`
		UNION ALL
		SELECT DISTINCT `id`, 
			CASE 
				WHEN `data_flags` LIKE '%IS_INTERIOR%' THEN `name`
				WHEN `name`!='' THEN CONCAT(`name`, ' ', `grid`)
				WHEN `region`!='' THEN CONCAT(`region`, ' ', `grid`)
				ELSE `grid` 
			END AS 'name', 
			'cell' AS `type` 
			FROM `cell`
		UNION ALL
		SELECT DISTINCT `id`, `name`, 'container' AS `type` FROM `container`
		UNION ALL
		SELECT DISTINCT `id`, `id` AS 'name', 'leveleditem' AS `type` FROM `leveleditem`
		UNION ALL
		SELECT DISTINCT `id`, `id` AS 'name', 'leveledcreature' AS `type` FROM `leveledcreature`
	;";

    /// <summary>
    /// Used to create links to object pages when rendering inventory lists
    /// </summary>
    public const string inventory_object_query = @"SELECT id, name, 'npc' AS type FROM npc	
		UNION ALL	
		SELECT id, name, 'book' AS type FROM book 
		WHERE 
			name!='' 
			AND icon NOT IN ('','default icon.dds') 
			AND mesh !='tr	r_editormarker_npc.nif'
			AND text != ''
		UNION ALL
		SELECT DISTINCT id, name, 'clothing' AS type FROM clothing
		UNION ALL
		SELECT DISTINCT id, name, 'armor' AS type FROM armor
		UNION ALL
		SELECT DISTINCT id, name, 'spell' AS type FROM spell
		UNION ALL
		SELECT DISTINCT id, name, 'weapon' AS type FROM weapon
		UNION ALL
		SELECT DISTINCT id, name, 'creature' AS type FROM creature
		UNION ALL
		SELECT DISTINCT id, name, 'miscitem' AS type FROM miscitem
		UNION ALL
		SELECT DISTINCT id, name, 'apparatus' AS type FROM apparatus
		UNION ALL
		SELECT DISTINCT id, name, 'probe' AS type FROM probe
		UNION ALL
		SELECT DISTINCT id, name, 'lockpick' AS type FROM lockpick
		UNION ALL
		SELECT DISTINCT id, name, 'repairitem' AS type FROM repairitem
		UNION ALL
		SELECT DISTINCT id, name, 'alchemy' AS type FROM alchemy
		UNION ALL
		SELECT DISTINCT id, name, 'ingredient' AS type FROM ingredient
		UNION ALL
		SELECT DISTINCT id, 
			CASE 
				WHEN data_flags LIKE '%IS_INTERIOR%' THEN name
				WHEN name!='' THEN CONCAT(name, ' ', grid)
				WHEN region!='' THEN CONCAT(region, ' ', grid)
				ELSE grid 
			END AS 'name', 
			'cell' AS type 
			FROM cell
		UNION ALL
		SELECT DISTINCT id, id AS 'name', 'leveleditem' AS type FROM leveleditem
		UNION ALL
		SELECT DISTINCT id, id AS 'name', 'leveledcreature' AS type FROM leveledcreature
	;";

    /// <summary>
    /// Returns meta data about the game files used to populate the database
    /// </summary>
    public const string header_query = "SELECT `expansion`, `author`, `description` FROM `header`;";
    #region Item List Queries
    public const string expansion_list_query = "SELECT DISTINCT `expansion` FROM `header`";

	public const string race_list_query = "SELECT DISTINCT `name` FROM `race` ORDER BY `name` ASC";

	public const string class_list_query = "SELECT DISTINCT `name` FROM `class` ORDER BY `name` ASC";

	public const string faction_list_query = "SELECT DISTINCT `name` FROM `faction` ORDER BY `name` ASC";

	public const string magic_effect_list_query = "SELECT DISTINCT `id` FROM `magiceffect` ORDER BY `id` ASC";
	
	public const string location_list_query = "SELECT DISTINCT `expansion`, `location` FROM `npc` ORDER BY `expansion`, `location`";
    #endregion

    #region Item Inventory Queries
    /* Queries that find objects (npcs, creatures, cells, etc) that hold a specific item */
    public const string npc_inventory_query = """
		SELECT 
			n.id, 
			n.name, 
			n.expansion, 
			n.location 
		FROM 
			npc n 
			JOIN json_each(n.inventory) AS j ON j.value->>'$.id' = @id 
		GROUP BY n.id 
		ORDER BY n.expansion, n.location, n.name;
		""";

    public const string creature_inventory_query = """
		SELECT 
			c.id,
			c.name,
			c.expansion,
			c.creature_type 
		FROM creature c
		JOIN json_each(c.inventory) AS j ON j.value->>'$.id' = @id 
		GROUP BY c.id 
		ORDER BY c.expansion, c.name;
		""";

    public const string container_inventory_query = @"
        SELECT c.id, c.name 
        FROM container c
        JOIN json_each(c.inventory) AS j 
          ON j.value->>'$.id' = @id
        GROUP BY c.id
        ORDER BY c.name;";

    public const string cell_inventory_query = @"
        SELECT a.id, a.region,
            CASE
                WHEN a.name != '' THEN a.name
                ELSE a.region || a.id
            END AS name 
        FROM cell AS a
        JOIN cellreferencemap AS c ON c.cell_id = a.id
        WHERE c.reference_id = @id
        ORDER BY name;";

    public const string item_list_query = @"
		SELECT l.id, l.id AS 'name', 'leveleditem' AS 'type', `expansion` 
		FROM leveleditem l
			JOIN json_each(l.items) AS j ON j.value->>'$.id' = @id
	    ORDER BY expansion, l.id;
	";
	#endregion

	#region Item Queries
	public const string alchemy_query = @"SELECT * FROM alchemy WHERE id = @id";
	public const string apparatus_query = @"SELECT * FROM apparatus WHERE id = @id";
	public const string armor_query = @"SELECT * FROM armor WHERE id = @id";
	public const string birthsign_query = @"SELECT * FROM birthsign WHERE id = @id";
	public const string birthsign_spell_query = @"SELECT s.* FROM birthsign b LEFT JOIN spell s ON EXISTS (SELECT 1 FROM json_each(b.spells) AS j WHERE j.value = s.id) WHERE b.id = @id;";
    public const string book_query = @"SELECT book.*, e.effects, e.enchant_type, e.cost, e.max_charge FROM book	LEFT JOIN enchanting AS e ON e.id = book.enchanting	WHERE book.id = @id";
    public const string cell_query = @"SELECT * FROM cell WHERE id = @id";
    public const string class_query = @"SELECT * FROM class WHERE id = @id";
    public const string clothing_query = @"SELECT c.*, e.effects, e.enchant_type, e.cost, e.max_charge FROM clothing AS c LEFT JOIN enchanting AS e ON e.id = c.enchanting WHERE c.id = @id";
	public const string container_query = @"SELECT * FROM container WHERE id = @id";
    public const string creature_query = @"SELECT * FROM creature WHERE id = @id";
	public const string faction_query = @"SELECT * FROM faction WHERE id = @id";
	public const string ingredient_query = @"SELECT * FROM ingredient WHERE id = @id";
	public const string leveledcreature_query = @"SELECT * FROM leveledcreature WHERE id = @id";
	public const string leveleditem_query = @"SELECT * FROM leveleditem WHERE id = @id";
	public const string lockpick_query = @"SELECT * FROM lockpick WHERE id = @id";
	public const string magiceffect_query = @"SELECT * FROM magiceffect WHERE id = @id";
    public const string npc_query = @"SELECT * FROM npc WHERE id = @id";
	public const string race_query = @"SELECT * FROM race WHERE id = @id";
    #endregion Item Queries

    #region Cell Queries
    public const string cell_npc_query = @"
	SELECT n.id, n.name, n.expansion, n.faction_id, n.faction
	FROM
		cellreferencemap AS c
		JOIN npc AS n ON c.reference_id = n.id
	WHERE c.cell_id = @id
	";

    public const string cell_equipment_query = @"
	SELECT clothing.id, clothing.name, clothing.clothing_type AS 'type', 'clothing' AS 'object', clothing.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		clothing
		JOIN cellreferencemap AS c ON c.reference_id = clothing.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id1
	UNION ALL
	SELECT w.id, w.name, w.weapon_type AS 'type', 'weapon' AS 'object', w.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		weapon AS w
		JOIN cellreferencemap AS c ON c.reference_id = w.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id2
	UNION ALL
	SELECT a.id, a.name, a.armor_type AS 'type', 'armor' AS 'object', a.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		armor AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id3

	ORDER BY 4, 3, 2;
	";

    public const string cell_ingredient_query = @"
	SELECT i.id, i.name, 'ingredient' AS 'object', i.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		ingredient AS i
		JOIN cellreferencemap AS c ON c.reference_id = i.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id1
	UNION ALL
	SELECT a.id, a.name, 'alchemy' AS 'object', a.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		alchemy AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id2
	ORDER BY 3, 2;
	";

    public const string cell_miscitem_query = @"
	SELECT m.id, m.name, 'miscitem' AS 'object', m.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		miscitem AS m
		JOIN cellreferencemap AS c ON c.reference_id = m.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id1
	UNION ALL
	SELECT m.id, m.name, 'book' AS 'object', m.expansion, c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		book AS m
		JOIN cellreferencemap AS c ON c.reference_id = m.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id2

	ORDER BY 3, 2;
	";

    public const string cell_tool_query = @"
	SELECT a.id, a.name, 'apparatus' AS 'object', c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		apparatus AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id1
	UNION ALL
	SELECT a.id, a.name, 'lockpick' AS 'object', c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		lockpick AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id2
	UNION ALL
	SELECT a.id, a.name, 'probe' AS 'object', c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		probe AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id3
	UNION ALL
	SELECT a.id, a.name, 'repairitem' AS 'object', c.quantity, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		repairitem AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id4

	ORDER BY 3, 2;
	";

    public const string cell_container_query = @"
	SELECT a.*, c.owner, c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name'
    FROM
		container AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id
	ORDER BY name;
	";

    public const string cell_cell_query = @"
	SELECT a.id, a.name, c.owner, 'cell' AS 'object', c.lock_level, c.trap, c.key, c.owner_faction, c.owner_faction_rank, npc.name AS 'owner_name', a.region
    FROM
		cell AS a
		JOIN cellreferencemap AS c ON c.reference_id = a.id
		LEFT JOIN npc ON npc.id = c.owner
    WHERE c.cell_id = @id
	ORDER BY name;
	";

    public const string cell_creature_query = @"
	SELECT n.id, n.name, n.expansion, n.creature_type, 'creature' AS 'object', c.quantity
	FROM
		cellreferencemap AS c
		JOIN creature AS n ON c.reference_id = n.id
	WHERE c.cell_id = @id
	";

    public const string cell_leveledcreature_query = @"
	SELECT l.id, l.id AS 'name', l.expansion, 'leveledcreature' AS 'object', c.quantity
	FROM
		cellreferencemap AS c
		JOIN leveledcreature AS l ON c.reference_id = l.id
	WHERE c.cell_id = @id";
    #endregion Cell Queries

    #region Class Faction Race Creature Queries
    public const string npc_class_query = @"
		SELECT id, name, expansion, location 
		FROM npc 
		WHERE class_id = @id 
		ORDER BY expansion, location, name;
		";

    public const string npc_faction_query = @"
		SELECT id, name, expansion, location 
		FROM npc 
		WHERE faction_id = @id 
		ORDER BY expansion, location, name;
		";

    public const string npc_race_query = @"
		SELECT id, name, expansion, location 
		FROM npc 
		WHERE race_id = @id 
		ORDER BY expansion, location, name;
		";

    public const string creature_list_query = @"
		SELECT id, id AS 'name', 'leveledcreature' AS 'type', expansion 
		FROM leveledcreature 
		WHERE EXISTS (
			SELECT 1 FROM json_each(creatures) AS j WHERE j.value->>'$.id' = @id
		)
		ORDER BY expansion, id;";
    #endregion

    #region Magic Effect Queries

    public const string spell_effect_query = @"
		SELECT id, name, spell_type, cost, expansion 
		FROM spell 
		WHERE EXISTS (
			SELECT 1 FROM json_each(effects) AS j WHERE j.value->>'$.magic_effect' = @id
		)
		ORDER BY name;
		";

    public const string clothing_effect_query = @"
		SELECT clothing.id, clothing.name, clothing.clothing_type AS 'type', 'clothing' AS 'object', expansion
		FROM clothing JOIN enchanting ON enchanting.id = clothing.enchanting
		WHERE EXISTS (
			SELECT 1 FROM json_each(enchanting.effects) AS j WHERE j.value->>'$.magic_effect' = @id
		)
		ORDER BY clothing.clothing_type, clothing.name;
		";

    public const string armor_effect_query = @"
		SELECT armor.id, armor.name, armor.armor_type AS 'type', 'armor' AS 'object', expansion
		FROM armor JOIN enchanting ON enchanting.id = armor.enchanting
		WHERE EXISTS (
			SELECT 1 FROM json_each(enchanting.effects) AS j WHERE j.value->>'$.magic_effect' = @id
		)
		ORDER BY armor.armor_type, armor.name;
		";

    public const string weapon_effect_query = @"
		SELECT weapon.id, weapon.name, weapon.weapon_type AS 'type', 'weapon' AS 'object', expansion
		FROM weapon JOIN enchanting ON enchanting.id = weapon.enchanting
		WHERE EXISTS (
			SELECT 1 FROM json_each(enchanting.effects) AS j WHERE j.value->>'$.magic_effect' = @id
		)
		ORDER BY weapon.weapon_type, weapon.name;
		";

    public const string alchemy_effect_query = @"
		SELECT id, name, 'alcehmy' AS object, effects, expansion 
		FROM alchemy 
		WHERE EXISTS (
			SELECT 1 FROM json_each(effects) AS j WHERE j.value->>'$.magic_effect' = @id
		)
		ORDER BY name;
		";

    public const string ingredient_effect_query = @"
		SELECT id, name, 'ingredient' AS object, effects, skills, attributes, expansion 
		FROM ingredient 
		WHERE EXISTS (
			SELECT 1 FROM json_each(effects) AS j WHERE j.value = @id
		)
		ORDER BY name;
		";

    #endregion

    public const string container_cell_query = @"
		SELECT 
			m.quantity, 
			m.owner, 
			m.lock_level, 
			m.key, 
			m.trap, 
			m.owner_faction, 
			m.owner_faction_rank,
			CASE
				WHEN c.name!='' THEN c.name
				ELSE c.id
			END AS name,
			c.id,
			c.region,
			'cell' AS object,
			npc.name AS 'owner_name'

		FROM
			cellreferencemap AS m
			JOIN cell AS c ON c.id = m.cell_id
			LEFT JOIN npc ON npc.id = m.owner
		WHERE reference_id = @id
		";
    /// <summary>
    /// Parameters @speakerId, @speakerId2, @speakerId3 should all be passed the same value
    /// </summary>
    public const string npc_dialogue_query = @"
    WITH RECURSIVE ordered_list AS (
		SELECT 
			id, 
			prev_id, 
			next_id,
			text,
			dialogue_type,
			dialogue_topic,
			dialogue_id,
			id AS chain_start,
			1 AS position,
			speaker_race,
			speaker_class,
			speaker_faction,
			speaker_cell,
			player_faction,
			disposition,
			speaker_rank,
			speaker_sex,
			player_rank,
			quest_state
		FROM dialogueinfo
		WHERE 
			speaker_id = @speakerId
			AND id NOT IN (
				SELECT next_id FROM dialogueinfo WHERE next_id IS NOT NULL
				AND speaker_id = @speakerId2
			)
		UNION ALL
		SELECT 
			d.id, 
			d.prev_id, 
			d.next_id,
			d.text,
			d.dialogue_type,
			d.dialogue_topic,
           	d.dialogue_id,
			ol.chain_start,
			ol.position + 1,
			d.speaker_race,
			d.speaker_class,
			d.speaker_faction,
			d.speaker_cell,
			d.player_faction,
			d.disposition,
			d.speaker_rank,
			d.speaker_sex,
			d.player_rank,
			d.quest_state
		FROM 
			dialogueinfo d
			JOIN ordered_list ol ON d.id = ol.next_id
		WHERE speaker_id = @speakerId3
		)
		SELECT DISTINCT dialogue_type, dialogue_topic, text, speaker_race, speaker_class, speaker_faction, speaker_cell, player_faction,
			disposition, speaker_rank, speaker_sex, player_rank, quest_state
		FROM ordered_list
		ORDER BY dialogue_type, chain_start, position DESC; ";
}

