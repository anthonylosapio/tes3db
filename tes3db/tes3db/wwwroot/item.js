function fetchObjectData(action) {

	//Add spinners to all the content divs while waiting for API response
	const contentDivs = document.querySelectorAll('.content.content-border.p-2');
	contentDivs.forEach(element => {
		element.append(createDiv(buildSpinner(), ['spinner-overlay']));
	});

	const queryString = window.location.search;
	const urlParams = new URLSearchParams(queryString);
	const id = urlParams.get('id')
	const url = `/api/${action}/${id}`;

	callServer(url, function (data) { start(data, action) });
}

function start(json, action) {
	document.querySelectorAll('.spinner-overlay').forEach(el => el.remove());

	if (json != null) {
		//console.log(json)
		const data = JSON.parse(json);

		renderPage(data.data[0], action);

		//check of object has spells property (birthsign) and render that if exists
		if (data.spells) {
			renderAbilities(data.spells);
		}

		//List linked NPCs
		//Used by Apparatus, Armor, Book, Clothing, Lockpick, MiscItem, Probe, RepairItem, Spell, Weapon
		const NpcLinkListId = document.getElementById('NpcLinkListId');
		if (data.npcs && NpcLinkListId) {
			renderLinkList(data.npcs, NpcLinkListId, 'npc');
		}

		//List linked Creatures
		//Used by Apparatus, Armor, Book, Clothing, Lockpick, MiscItem, Probe, RepairItem, Spell, Weapon
		const CreatureLinkListId = document.getElementById('CreatureLinkListId');
		if (data.creatures && CreatureLinkListId) {
			//console.log(JSON.stringify(data.creatures));
			renderLinkList(data.creatures, CreatureLinkListId, 'creature');
		}

		//List linked Cells
		//Used by Apparatus, Armor, Book, Clothing, Lockpick, MiscItem, Probe, RepairItem, Spell, Weapon
		const ContainerLinkListId = document.getElementById('ContainerLinkListId');
		if (data.containers && ContainerLinkListId) {
			renderLinkList(data.containers, ContainerLinkListId, 'container');
		}

		//List linked Cells
		//Used by Apparatus, Armor, Book, Clothing, Lockpick, MiscItem, Probe, RepairItem, Spell, Weapon
		const CellLinkListId = document.getElementById('CellLinkListId');
		if (data.cells && CellLinkListId) {
			renderLinkList(data.cells, CellLinkListId, 'cell');
		}

		//List Linked Ingredients by effect
		//Used by MagicEffect
		const IngredientsEffectContainer = document.getElementById('IngredientsEffectContainer');
		if (data.ingredientsEffect && IngredientsEffectContainer) {
			renderLinkList(data.ingredientsEffect, IngredientsEffectContainer, 'ingredient');
		}

		//List Linked Alchemy by effect
		//Used by MagicEffect
		const AlchemyEffectContainer = document.getElementById('AlchemyEffectContainer');
		if (data.alchemyEffect && AlchemyEffectContainer) {
			renderLinkList(data.alchemyEffect, AlchemyEffectContainer, 'alchemy');
		}

		//List Linked Spells by effect
		//Used by MagicEffect
		const SpellsEffectContainer = document.getElementById('SpellsEffectContainer');
		if (data.spellsEffect && SpellsEffectContainer) {
			renderLinkList(data.spellsEffect, SpellsEffectContainer, 'spell');
		}

		//List Linked Clothing by enchantment effect
		//Used by MagicEffect
		const ClothingContainer = document.getElementById('ClothingContainer');
		if (data.clothing && ClothingContainer) {
			renderLinkList(data.clothing, ClothingContainer, 'clothing');
		}

		//List of Armor
		//Used by MagicEffect
		const ArmorContainer = document.getElementById('ArmorContainer');
		if (data.armor && ArmorContainer) {
			renderLinkList(data.armor, ArmorContainer, 'armor');
		}

		//List of Weapons
		//Used by MagicEffect
		const WeaponsContainer = document.getElementById('WeaponsContainer');
		if (data.weapons && WeaponsContainer) {
			renderLinkList(data.weapons, WeaponsContainer);
		}

		//List of Ingredients
		//Used by Cell
		const IngredientsContainer = document.getElementById('IngredientsContainer');
		if (data.ingredients && IngredientsContainer) {
			renderTableBodyRows(data.ingredients, IngredientsContainer);
		}

		//List of Tools (Apparatus, Lockpicks, Probes, RepairItems)
		//Used by Cell
		const ToolsContainer = document.getElementById('ToolsContainer');
		if (data.tools && ToolsContainer) {
			renderTableBodyRows(data.tools, ToolsContainer);
		}

		//List of Misc Items
		//Used by Cell
		const MiscItemContainer = document.getElementById('MiscItemContainer');
		if (data.miscitems && MiscItemContainer) {
			//			console.log(JSON.stringify(data.miscitems));
			renderTableBodyRows(data.miscitems, MiscItemContainer);
		}

		//List of Misc Items
		//Used by Cell
		const EquipmentContainer = document.getElementById('EquipmentContainer');
		if (data.equipment && EquipmentContainer) {
			renderTableBodyRows(data.equipment, EquipmentContainer);
		}
		//List of Misc Items
		//Used by Cell
		const CellsContainer = document.getElementById('CellsContainer');

		if (data.cells && CellsContainer) {
			//			console.log(JSON.stringify(data.cells));
			renderTableBodyRows(data.cells, CellsContainer);
		}

		//List of Containers
		//Used by Cell
		const ContainerContainer = document.getElementById('ContainerContainer');
		if (data.containers && ContainerContainer) {
			renderContainers(data.containers, ContainerContainer);
		}

		//List Dialogue grouped by Topic
		//Used by NPC
		const dialogueDiv = document.getElementById('DialogueDiv');
		if (dialogueDiv && data.dialogue) {
			renderTopic(dialogueDiv, data.dialogue);
		}



		/*!!!!!!!!!!!!!! tes3db.com only */
		document.getElementById('ViewCountNameId').textContent = data.data[0].name;
		document.getElementById('ViewCountId').textContent = data.count;
		/*!!!!!!!!!!!!!! tes3db.com only */
	}

}


//Propterties of data are rendered here
function renderPage(data, action) {
	//console.log(JSON.stringify(data));
	for (const [key, value] of Object.entries(data)) {

		switch (key) {
			case 'text':
				let val = value.replace('<FONT COLOR="000000" SIZE="3" FACE="Magic Cards">', '');
				val = val.replace('<FONT COLOR="000000" SIZE="2" FACE="Daedric">', '');
				val = val.replace('<FONT COLOR="000000" SIZE="2" FACE="Magic Cards">', '');
				val = val.replace('<FONT COLOR="640F0F" SIZE="3" FACE="Magic Cards">', '');
				val = val.replace('<FONT COLOR="000000" SIZE="3" FACE="Magic Cards">', '');
				val = val.replace('</FONT>', '');
				val = val.replace('</font>', '');

				renderProperty_innerHTML(key, val);
				break;

			case 'items':
			case 'inventory':
				const InventoryTableId = document.getElementById('InventoryTableId')
				if (InventoryTableId) renderMixedTable(value, InventoryTableId);
				break;

			case 'spells':
				const SpellsTableId = document.getElementById('SpellsTableId');
				if (SpellsTableId) renderSpells(value, SpellsTableId);
				break;

			case 'flags':
			case 'npc_flags':
			case 'data_flags':
			case 'creature_flags':
			case 'container_flags':
			case 'leveled_item_flags':
			case 'services':
				newValue = value.replaceAll('|', '<br>').replace('0xa00000', '').replace('0x1c80000', '');
				renderProperty_innerHTML(key, newValue);
				break;

			case 'effects':
				if (value != null && action != 'Ingredient') {
					const enchantmentDiv = document.getElementById('EnchantmentsDivId');
					if (enchantmentDiv) enchantmentDiv.classList.remove('d-none');
					const magicEffectsDiv = document.getElementById('MagicEffectsDivId');
					renderEffects(value, magicEffectsDiv, action);
				}
				if (action == 'Ingredient') {
					renderIngredientEffects(JSON.parse(data.effects), JSON.parse(data.skills), JSON.parse(data.attributes));
				}
				break;

			case 'class':
				document.getElementById('classId').append(buildObjectLinkButton(value, value, 'class', 'linkBtn'));
				break;

			case 'race':
				document.getElementById('raceId').append(buildObjectLinkButton(value, value, 'race', 'linkBtn'));
				break;

			case 'faction':
				document.getElementById('factionId').append(buildObjectLinkButton(value, value, 'faction', 'linkBtn'));
				break;

			case 'favored_skills':
				const favored_skillsId = document.getElementById('favored_skillsId');
				if (favored_skillsId) renderLinkStringArray(JSON.parse(value), favored_skillsId, 'skill');
				break;

			case 'requirements':
				const ranksId = document.getElementById('ranksId');
				if (ranksId) renderRanksandRequirements(JSON.parse(data.rank_names), JSON.parse(data[key]), ranksId);
				break;

			case 'reactions':
				const reactionsId = document.getElementById('reactionsId');
				if (reactionsId) renderTableBodyRows(JSON.parse(value), reactionsId);
				break;

			case 'cell':
				document.getElementById('cellId').append(buildObjectLinkButton(value, data.cell_id, 'cell', 'linkBtn'));
				break;

			default:
				renderProperty(key, value);
				break;
		}

	}

}

function renderProperty(key, val) {
	const elementId = `${key}Id`;
	const element = document.getElementById(elementId);

	if (element) {
		element.textContent = val;
	}
}

function renderProperty_innerHTML(key, val) {
	const elementId = `${key}Id`;
	const element = document.getElementById(elementId);

	if (element) {
		element.innerHTML = val;
	}
}

async function renderMixedTable(data, tbody) {

	//get the list of id'd items so we can link to book/clothing/weapon/armor pages
	const idbObj = await idbSearchCache.load(cacheKey);
	const objList = JSON.parse(idbObj);
	const filteredList = objList.filter(ol => ['alchemy', 'apparatus', 'armor', 'book', 'clothing', 'creature', 'ingredient', 'leveleditem', 'lockpick', 'miscitem', 'probe', 'repairitem', 'weapon', 'npc', 'cell'].includes(ol.type));

	const obj = JSON.parse(data);
	tbody.innerHTML = '';

	const nameCounts = {};
	for (const item of obj) {
		const match = filteredList.find(fl => fl.id === item.id);
		if (match) nameCounts[match.name] = (nameCounts[match.name] || 0) + 1;
	}

	for (const item of obj) {

		const row = document.createElement('tr');

		//Handles NPC & Container inventory items which have id and quantity properties
		if (item.id) {
			const match = filteredList.find(fl => fl.id === item.id);
			if (match) {
				const isDuplicateName = nameCounts[match.name] > 1;
				row.append(createTd(buildObjectLinkButton(match.name, match.id, match.type, 'linkBtn', isDuplicateName)));
			} else {
				row.append(createTd(item.id));
			}
			if (match && !isNullOrEmpty(match.type)) {
				row.appendChild(createTd(match.type, []));
			} else {
				row.appendChild(createTd('', []));
			}
			row.appendChild(createTd(item.quantity, ['text-center']));
		} else if (item) {
			match = filteredList.find(fl => fl.id === item);
			if (match) {
				row.append(
					createTd(
						buildObjectLinkButton(match.name, match.id, match.type, 'linkBtn', isDuplicateName))
				);
				row.append(
					createTd(match.type)
				);
			} else {
				//	row.append(item);
			}
		}

		tbody.appendChild(row);
	}
}

function renderSpells(data, tbody) {
	const obj = JSON.parse(data);

	tbody.innerHTML = '';

	for (const item of obj) {
		const row = document.createElement('tr');
		const spellCell = document.createElement('td');
		spellCell.append(buildObjectLinkButton(item, item, 'spell'));
		row.appendChild(spellCell);
		tbody.appendChild(row);
	}

	if (obj.length === 0) {
		const row = document.createElement('tr');
		const spellCell = document.createElement('td');
		spellCell.textContent = 'none';
		row.appendChild(spellCell);
		tbody.appendChild(row);
	}
}

//Renders a list of objects with id & name that link to other object page
function renderLinkList(val, parent, type) {

	//console.log(JSON.stringify(val));

	const table = document.createElement('table');
	table.classList.add('w-100');
	parent.append(table);

	const nameCounts = {};
	for (const obj of val) {
		nameCounts[obj.name] = (nameCounts[obj.name] || 0) + 1;
	}

	for (const obj of val) {

		const isDuplicateName = nameCounts[obj.name] > 1;
		const tr = document.createElement('tr')
		const td = document.createElement('td');
		const button = buildObjectLinkButton(obj.name, obj.id, type, 'linkBtn', isDuplicateName);
		td.append(button);
		tr.append(td);

		if (obj.location) {
			tr.append(createTd(obj.location));
		}

		if (obj.type) {
			tr.append(createTd(obj.type));
		}

		if (obj.spell_type) {
			tr.append(createTd(obj.spell_type));
		}

		if (obj.creature_type) {
			tr.append(createTd(obj.creature_type));
		}

		if (obj.object) {
			tr.append(createTd(obj.object));
		}

		if (obj.region) {
			tr.append(createTd(obj.region));
		}

		if (obj.quantity) {
			tr.append(createTd(obj.quantity));
		}

		if (obj.faction && obj.faction_id) {
			tr.append(createTd(buildObjectLinkButton(obj.faction, obj.faction_id, 'faction', 'linkBtn')));
		}

		if (obj.expansion) {
			tr.append(createTd(obj.expansion));
		}

		//ADD MORE CUSTOMER COLUMNS HERE

		table.append(tr);
	}
}

function renderLinkStringArray(array, parent, type) {
	const table = document.createElement('table');
	table.classList.add('w-100');
	parent.append(table);

	for (const obj of array) {
		const tr = document.createElement('tr')
		const td = document.createElement('td');
		const button = buildObjectLinkButton(obj, obj, type)
		td.append(button);
		tr.append(td);
		table.append(tr);
	}
}

function renderTableBodyRows(object, parent) {
	//	console.log(JSON.stringify(object));
	const nameCounts = {};
	let hasOwner = false;
	let hasKey = false;
	let hasRegion = false;
	for (const obj of object) {
		nameCounts[obj.name] = (nameCounts[obj.name] || 0) + 1;
		if (obj.owner || obj.owner_faction) hasOwner = true;
		if (obj.lock_level || obj.trap) hasKey = true;
		if (obj.region) hasRegion = true;
	}

	if (object[0] && object[0].object && object[0].object == 'cell') {
		const headTr = document.createElement('tr');
		headTr.append(createTd('cell', ['table-label']));
		if (hasRegion) headTr.append(createTd('region', ['table-label']));
		if (hasKey) headTr.append(createTd('lock/trap', ['table-label']));
		if (hasOwner) headTr.append(createTd('owner', ['table-label']));
		headTr.append(createTd('quantity', ['table-label']));
		parent.append(headTr);
	}

	for (const row of object) {

		const isDuplicateName = nameCounts[row.name] > 1;

		const tr = document.createElement('tr');

		//rows for Faction page reactions
		if (row.faction) tr.append(createTd(buildObjectLinkButton(row.faction, row.faction, 'faction')));
		if (row.reaction) tr.append(createTd(row.reaction, ['text-center']));


		if (row.name && row.id && row.object) tr.append(createTd(buildObjectLinkButton(row.name, row.id, row.object, 'linkBtn', isDuplicateName)));

		if (hasRegion) tr.append(createTd(row.region));

		if (hasKey) {
			const lockCell = createTd('');

			if (row.lock_level) {
				lockCell.append(
					createSpan(`lock:`, ['small', 'table-label']),
					createSpan(`${row.lock_level}`, ['small', 'table-value'])
				);
			}
			if (row.key) {
				lockCell.append(createSpan(buildObjectLinkButton('key', row.key, 'miscitem', 'linkBtn'), ['small', 'ms-1']));
			}
			if (!isNullOrEmpty(row.trap)) {
				lockCell.append(
					//createSpan(` - `),
					createSpan(`trap:`, ['table-label', 'small']),
					createSpan(`${row.trap}`)
				);
			}
			tr.append(lockCell);

		}

		if (row.key && row.object != 'cell') tr.append(createTd(buildObjectLinkButton(row.key, row.key, 'miscitem', 'linkBtn')));

		if (row.object && row.object != 'cell') tr.append(createTd(row.object));

		if (hasOwner /*&& row.object!='cell'*/) {
			if (row.owner) {
				tr.appendChild(createTd(buildObjectLinkButton(row.owner_name, row.owner, 'npc', 'linkBtn')));
			} else if (row.owner_faction) {
				if (row.owner_faction) tr.appendChild(createTd(buildObjectLinkButton(row.owner_faction, row.owner_faction, 'faction', 'linkBtn')));
			} else {
				tr.appendChild(createTd(''));
			}
		}

		if (row.quantity) tr.append(createTd(row.quantity, ['text-center']));


		parent.append(tr);
	}
}

function renderContainers(object, parent) {
	//console.log(JSON.stringify(object));
	for (const row of object) {

		const div = createDiv('');
		parent.append(div);

		if (row.name && row.id) {

			div.append(
				createSpan(row.name, ['table-value', 'fw-semibold']),
				createSpan(`${row.id}`, ['table-label', 'small', 'ms-1']),
			);

			if (!isNullOrEmpty(row.owner)) {
				div.append(
					createSpan(` - `),
					createSpan(`owner:`, ['table-label', 'small']),
					buildObjectLinkButton(row.owner_name, row.owner, 'npc', 'linkBtn')
				);
			}

			if (!isNullOrEmpty(row.owner_faction)) {
				div.append(
					createSpan(` - `),
					createSpan(`owner:`, ['table-label', 'small']),
					buildObjectLinkButton(row.owner_faction, row.owner_faction, 'faction', 'linkBtn'),
					createSpan(`rank ${row.owner_faction_rank}`, ['table-label', 'small'])
				);
			}

			if (!isNullOrEmpty(row.lock_level)) {
				div.append(
					createSpan(` - `),
					createSpan(`lock:`, ['table-label', 'small']),
					createSpan(`${row.lock_level}`)
				);
			}

			if (!isNullOrEmpty(row.trap)) {
				div.append(
					createSpan(` - `),
					createSpan(`trap:`, ['table-label', 'small']),
					createSpan(`${row.trap}`)
				);
			}

			if (!isNullOrEmpty(row.key)) {
				div.append(
					createSpan(` - `),
					createSpan(`key:`, ['table-label', 'small']),
					buildObjectLinkButton(row.key, row.key, 'miscitem', 'linkBtn')
				);
			}

		}
		const table = document.createElement('table');

		const tbody = document.createElement('tbody');
		tbody.classList.add('container-tbody');
		table.append(tbody);

		if (row.inventory) renderMixedTable(row.inventory, tbody);

		parent.append(table);
	}
}

function renderRanksandRequirements(rankArray, reqObj, parent) {
	let i = 0;
	for (const rank of rankArray) {
		const tr = document.createElement('tr');

		tr.append(createTd(rank));

		if (reqObj[i].attributes) tr.append(createTd(reqObj[i].attributes, ['text-center']));
		if (reqObj[i].primary_skill) tr.append(createTd(reqObj[i].primary_skill, ['text-center']));
		if (reqObj[i].favored_skill) tr.append(createTd(reqObj[i].favored_skill, ['text-center']));
		if (reqObj[i].reputation) tr.append(createTd(reqObj[i].reputation, ['text-center']));

		parent.append(tr);
		i++;
	}
}

function renderEffects(val, parent, action) {
	//console.log(val);
	effects = JSON.parse(val);


	for (const effect of effects) {

		const table = document.createElement('table');
		table.classList.add('w-100');
		table.style.tableLayout = 'fixed';
		const tbody = document.createElement('tbody');

		const row1 = document.createElement('tr');
		const magicEffectCell = document.createElement('td');
		magicEffectCell.classList.add('table-label');
		magicEffectCell.textContent = 'Magic Effect:';
		const magicEffectValueCell = document.createElement('td');

		const button = buildObjectLinkButton(effect.magic_effect, effect.magic_effect, 'magiceffect', 'linkBtn');
		magicEffectValueCell.append(button);
		row1.append(magicEffectCell, magicEffectValueCell);
		tbody.append(row1);

		if (effect.skill != 'None') {
			const row2 = document.createElement('tr');
			const skillEffectCell = document.createElement('td');
			skillEffectCell.classList.add('table-label');
			skillEffectCell.textContent = 'Skill:';
			const skillEffectValueCell = document.createElement('td');
			skillEffectValueCell.classList.add('table-value');
			skillEffectValueCell.textContent = effect.skill;
			row2.append(skillEffectCell, skillEffectValueCell);
			tbody.append(row2);
		}

		if (effect.attribute != 'None') {
			const row3 = document.createElement('tr');
			const attributeEffectCell = document.createElement('td');
			attributeEffectCell.classList.add('table-label');
			attributeEffectCell.textContent = 'Attribute:';
			const attributeEffectValueCell = document.createElement('td');
			attributeEffectValueCell.classList.add('table-value');
			attributeEffectValueCell.textContent = effect.attribute;
			row3.append(attributeEffectCell, attributeEffectValueCell);
			tbody.append(row3);
		}

		const row4 = document.createElement('tr');
		const rangeEffectCell = document.createElement('td');
		rangeEffectCell.classList.add('table-label');;
		rangeEffectCell.textContent = 'Range:';
		const rangeEffectValueCell = document.createElement('td');
		rangeEffectValueCell.classList.add('table-value');
		rangeEffectValueCell.textContent = effect.range;
		row4.append(rangeEffectCell, rangeEffectValueCell);

		const row5 = document.createElement('tr');
		const areaEffectCell = document.createElement('td');
		areaEffectCell.classList.add('table-label');
		areaEffectCell.textContent = 'Area:';
		const areaEffectValueCell = document.createElement('td');
		areaEffectValueCell.classList.add('table-value');
		areaEffectValueCell.textContent = effect.area;
		row5.append(areaEffectCell, areaEffectValueCell);

		const row6 = document.createElement('tr');
		const durationEffectCell = document.createElement('td');
		durationEffectCell.classList.add('table-label');
		durationEffectCell.textContent = 'Duration:';
		const durationEffectValueCell = document.createElement('td');
		durationEffectValueCell.classList.add('table-value');
		durationEffectValueCell.textContent = effect.duration;
		row6.append(durationEffectCell, durationEffectValueCell);

		const row7 = document.createElement('tr');
		const magEffectCell = document.createElement('td');
		magEffectCell.classList.add('table-label');
		magEffectCell.textContent = 'Magnitude:';
		const magEffectValueCell = document.createElement('td');
		magEffectValueCell.classList.add('table-value');
		magEffectValueCell.textContent = effect.min_magnitude + ' - ' + effect.max_magnitude;
		row7.append(magEffectCell, magEffectValueCell);

		tbody.append(row4, row5, row6, row7);
		table.append(tbody);

		const wrapper = document.createElement('div');
		if (action == 'Spell' || action == 'Birthsign') {
			wrapper.classList.add('p-2', 'mb-2', 'col-12');
		} else {
			wrapper.classList.add('p-2', 'mb-2', 'col-12', 'col-lg-6', 'col-xl-4');
		}

		wrapper.append(table);
		parent.append(wrapper);
	}
}

function renderIngredientEffects(effects, skills, attributes) {

	//console.log(JSON.stringify(skills));
	//console.log(JSON.stringify(attributes));
	const container = document.getElementById('IngredientEffectsId');

	let i = 0;
	while (i < 4) {

		const div = document.createElement('div');

		let buttonText = effects[i];

		if (buttonText.includes('Attribute')) {
			const replacementText = ' ' + attributes[i];
			buttonText = buttonText.replace('Attribute', replacementText);
		}

		if (buttonText.includes('Skill')) {
			const replacementText = ' ' + skills[i];
			buttonText = buttonText.replace('Skill', replacementText);
		}

		const linkBtn = buildObjectLinkButton(buttonText, effects[i], 'magiceffect');

		div.append(linkBtn);

		container.append(div);
		i++;
	}

}

function renderTopic(dialogueDiv, obj) {
	//console.log(JSON.stringify(obj));

	let currentTopic = "";

	const classList = ['col-auto', 'px-3', 'table-label']
	for (const item of obj) {

		if (item.dialogue_type == 'Greeting' && currentTopic != 'Greeting') {
			currentTopic = 'Greeting'

			const row = document.createElement('div');
			row.classList.add('row', 'g-0');

			const col = document.createElement('div');
			col.classList.add('col-12', 'dialogue-topic', 'pt-3', 'p-2');
			col.textContent = item.dialogue_type;

			row.append(col);
			dialogueDiv.appendChild(row);
		}

		if ((item.dialogue_type == 'Topic' || item.dialogue_type == 'Voice') && currentTopic != item.dialogue_topic) {
			currentTopic = item.dialogue_topic

			const row = document.createElement('div');
			row.classList.add('row', 'g-0');

			const col = document.createElement('div');
			col.classList.add('col-12', 'dialogue-topic', 'pt-3', 'p-2');
			col.append(buildObjectLinkButton(item.dialogue_topic, item.dialogue_topic, 'topic'));

			row.append(col);
			dialogueDiv.appendChild(row);
		}

		const row = document.createElement('div');
		row.classList.add('row', 'g-0', 'mt-3');
		if (!isNullOrEmpty(item.speaker_id)) row.append(createDiv(buildObjectLinkButton(item.speaker_id, item.speaker_id, 'npc', 'linkBtn'), classList));
		if (!isNullOrEmpty(item.speaker_race)) row.append(
			createDiv('speaker race:', classList),
			createDiv(buildObjectLinkButton(item.speaker_race, item.speaker_race, 'race', 'linkBtn'), classList)
		);
		if (!isNullOrEmpty(item.speaker_class)) row.append(
			createDiv('speaker class:', classList),
			createDiv(buildObjectLinkButton(item.speaker_class, item.speaker_class, 'class', 'linkBtn'), classList)
		);
		if (!isNullOrEmpty(item.speaker_faction)) row.append(
			createDiv('speaker faction:', classList),
			createDiv(buildObjectLinkButton(item.speaker_faction, item.speaker_faction, 'faction', 'linkBtn'), classList)
		);

		//if(!isNullOrEmpty(item.player_faction)) row.append(createDiv(buildObjectLinkButton(item.player_faction, item.player_faction, 'faction'), classList));

		if (!isNullOrEmpty(item.speaker_cell)) row.append(createDiv('speaker cell:' + item.speaker_cell, classList));
		if (!isNullOrEmpty(item.disposition)) row.append(createDiv('disposition:' + item.disposition, classList));
		if (!isNullOrEmpty(item.speaker_rank) && item.speaker_rank != -1) row.append(createDiv('speaker rank:' + item.speaker_rank, classList));
		//if(!isNullOrEmpty(item.speaker_sex)) row.append(createDiv('speaker sex:'+item.speaker_sex, classList));
		if (!isNullOrEmpty(item.player_rank) && item.player_rank != -1) row.append(createDiv('player rank:' + item.player_rank, classList));
		if (!isNullOrEmpty(item.quest_state)) row.append(createDiv('quest state:' + item.quest_state, classList));

		if (!isNullOrEmpty(item.expansion)) row.append(createDiv(item.expansion, classList));


		dialogueDiv.appendChild(row);

		const rowText = document.createElement('div');
		rowText.classList.add('row', 'g-0', 'ps-2');

		const colText = document.createElement('div');
		colText.classList.add('col-12', 'px-2');
		colText.textContent = item.text;

		rowText.append(colText);
		dialogueDiv.appendChild(rowText);
	}
}

function renderAbilities(data) {
	//	console.log(JSON.stringify(data));
	const container = document.getElementById('BirthsignAbilityDivId');
	for (const item of data) {
		//should contain poperties id, name, effects, spell_type, cost

		const button = buildObjectLinkButton(item.name, item.id, 'spell')
		const div = document.createElement('div');
		const table = document.createElement('table');
		const row1 = document.createElement('tr');
		const row2 = document.createElement('tr');
		const row3 = document.createElement('tr');
		const td = document.createElement('td');
		const td2label = document.createElement('td');
		const td2value = document.createElement('td');
		const td3label = document.createElement('td');
		const td3value = document.createElement('td');
		div.append(table);
		table.append(row1, row2, row3);
		row1.append(td);
		row2.append(td2label, td2value);
		row3.append(td3label, td3value);

		div.classList.add('col-12', 'px-2', 'm-0', 'mt-3');
		table.classList.add('w-100');
		table.style.tableLayout = 'fixed';

		td.colSpan = 2;
		td.classList.add('text-center');
		td2label.classList.add('table-label');
		td3label.classList.add('table-label');

		td2value.classList.add('table-value');
		td3value.classList.add('table-value');

		td2label.textContent = 'Type:';
		td3label.textContent = 'Cost:';

		td.append(button);
		td2value.textContent = item.spell_type;
		td3value.textContent = item.cost;

		const effectsDiv = document.createElement('div');
		effectsDiv.classList.add('col-12');
		renderEffects(item.effects, effectsDiv, 'birthsignData')

		container.append(div, effectsDiv)
	}
}
