using SQLitePCL;
using static tes3db.Models;

namespace tes3db;

using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class SqlQueries
{
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

    public const string header_query = "SELECT `expansion`, `author`, `description` FROM `header`;";

	public const string expansion_query = "SELECT DISTINCT `expansion` FROM `header`";

	public const string race_query = "SELECT DISTINCT `name` FROM `race` ORDER BY `name` ASC";

	public const string class_query = "SELECT DISTINCT `name` FROM `class` ORDER BY `name` ASC";

	public const string faction_query = "SELECT DISTINCT `name` FROM `faction` ORDER BY `name` ASC";

	public const string magic_effect_query = "SELECT DISTINCT `id` FROM `magiceffect` ORDER BY `id` ASC";
	
	public const string location_query = "SELECT DISTINCT `expansion`, `location` FROM `npc` ORDER BY `expansion`, `location`";

}

