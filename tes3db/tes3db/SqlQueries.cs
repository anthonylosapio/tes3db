using SQLitePCL;
using static tes3db.Models;

namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
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
	;";

    /// <summary>
    /// Returns meta data about the game files used to populate the database
    /// </summary>
    public const string header_query = "SELECT `expansion`, `author`, `description` FROM `header`;";

	public const string expansion_query = "SELECT DISTINCT `expansion` FROM `header`";

	public const string race_query = "SELECT DISTINCT `name` FROM `race` ORDER BY `name` ASC";

	public const string class_query = "SELECT DISTINCT `name` FROM `class` ORDER BY `name` ASC";

	public const string faction_query = "SELECT DISTINCT `name` FROM `faction` ORDER BY `name` ASC";

	public const string magic_effect_query = "SELECT DISTINCT `id` FROM `magiceffect` ORDER BY `id` ASC";
	
	public const string location_query = "SELECT DISTINCT `expansion`, `location` FROM `npc` ORDER BY `expansion`, `location`";

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
		ORDER BY c.expansion, c.location, c.name;
		""";

    public const string container_inventory_query = @"
        SELECT c.id, c.name 
        FROM container c
        JOIN json_each(c.inventory) AS j 
          ON j.value->>'$.id' = @itemId
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
        WHERE c.reference_id = @cellId
        ORDER BY name;";

    #endregion
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

