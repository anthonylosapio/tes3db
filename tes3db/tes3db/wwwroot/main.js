Object.keys(localStorage).filter(k => k.startsWith('searchObject_') && k !== cacheKey).forEach(k => localStorage.removeItem(k));
let trie;

function start() {

	if (!trie) trie = new Trie();

	callServer("/api/interface", function (json) {

		const data = JSON.parse(json);
		console.log(data);

		buildInterface(data);

		const s = { value: 'npcs' };
		changeObjectSelect(s);
		

		
		const queryCountId = document.getElementById('queryCountId');
		if (data.Count && queryCountId) queryCountId.textContent = data.Count.Total
	});

	callServer("/api/header", function (json) {
		const data = JSON.parse(json);
		const headerTableId = document.getElementById('headerTableId');
		if (data && headerTableId) {
			buildHeaderTable(headerTableId, data);
		}
	});

	// callServer("Posts", "", function (data) {
	// 	buildUpdatePosts(data)
	// });

	// callServer("TopList", "", function (data) {
	// 	//console.log(data);
	// 	buildTopList(data)
	// });

	//assign functions
	const getDataButton = document.getElementById('getDataButton');
	if (getDataButton) getDataButton.addEventListener('click', () => fetchData(getDataButton));

	const listNPCsButton = document.getElementById('listNPCsButton');
	if (listNPCsButton) listNPCsButton.addEventListener('click', () => fetchNPCList(listNPCsButton));

	const ObjectSelect = document.getElementById('ObjectSelectId');
	if (ObjectSelect) ObjectSelect.addEventListener('change', () => changeObjectSelect(ObjectSelect));

	document.querySelectorAll('.past-updates-btn').forEach(btn => {
		btn.addEventListener('click', () => pastUpdatesButtonClick(btn));
	});

	const objectSearchInput = document.getElementById('objectSearchInput');
	const objectSearchClear = document.getElementById('objectSearchClear');
	if (objectSearchInput)
		objectSearchInput.addEventListener(
			"input",
			debounce(() => {
				const query = document.getElementById('objectSearchInput').value.toLowerCase();
				const matches = trie.search(query, 200); //<---change value here for search results returned limit
				const resultsDiv = document.getElementById('searchResultsDiv');

				resultsDiv.innerHTML = '';

				matches.forEach(match => resultsDiv.append(
					buildObjectLinkButton(match.name, match.id, match.type, 'npcLinkBtn', true, true)
				));
			},
				120)
		);
	if (objectSearchInput) objectSearchInput.addEventListener(
		"keyup",
		() => {

			if (objectSearchInput.value.length == 0) {
				if (objectSearchClear) objectSearchClear.classList.add('d-none');
				setTimeout(() => {
					const resultsDiv = document.getElementById('searchResultsDiv');
					resultsDiv.innerHTML = '';
				}, 200);

			} else {
				if (objectSearchClear) objectSearchClear.classList.remove('d-none');
			}
		}
	);

	if (objectSearchClear) objectSearchClear.addEventListener('click', (event) => {
		objectSearchInput.value = '';
		objectSearchClear.classList.add('d-none');
		document.getElementById('searchResultsDiv').innerHTML = '';
		objectSearchInput.focus();
	});

	loadSeachObject();

}

function fetchData(btn) {

	const container = document.getElementById('ResultsDiv');
	container.innerHTML = '';
	const spinner = buildSpinner();
	container.append(spinner);

	const text = btn.querySelector('.btn-text');
	btn.disabled = true;

	const filters = getSelections();

	const aggValue = document.getElementById('AggSelectId').value;
	const sortValue = document.getElementById('SortBySelectId').value;
	const groupValue = document.getElementById('GroupBySelectId').value;
	const minValue = document.getElementById('minNpcSelectId').value;

	const selections = {
		agg: aggValue,
		sort: sortValue,
		group: groupValue,
		limit: '100',
		min: minValue,
		order: 'DESC',
	};

	const obj = {
		selections: selections,
		filters: filters
	};

	const json = JSON.stringify(obj);

	callServer("Query", json, function (data) { loadData(data, btn) });
}

function fetchNPCList(btn) {

	const ObjectSelected = document.getElementById('ObjectSelectId').value;
	let action = 'List';
	let table = 'npc'
	switch (ObjectSelected) {
		case 'npcs':
			action = 'List';
			break;
		case 'alchemy':
			action = 'ListAlchemy';
			break;
		case 'apparatuses':
			action = 'ListApparatuses';
			break;
		case 'armors':
			action = 'ListArmors';
			break;
		case 'books':
			action = 'ListBooks';
			break;
		case 'classes':
			action = 'ListClasses';
			break;
		case 'clothing':
			action = 'ListClothing';
			break;
		case 'creatures':
			action = 'ListCreatures';
			break;
		case 'factions':
			action = 'ListFactions';
			break;
		case 'ingredients':
			action = 'ListIngredients';
			break;
		case 'lockpicks':
			action = 'ListLockpicks';
			break;
		case 'magiceffects':
			action = 'ListMagicEffects';
			break;
		case 'miscitems':
			action = 'ListMiscItems';
			break;
		case 'probes':
			action = 'ListProbes';
			break;
		case 'races':
			action = 'ListRaces';
			break;
		case 'repairitems':
			action = 'ListRepairItems';
			break;
		case 'spells':
			action = 'ListSpells';
			break;
		case 'weapons':
			action = 'ListWeapons';
			break;
	}

	const container = document.getElementById('ResultsDiv');
	container.innerHTML = '';
	const spinner = buildSpinner();
	container.append(spinner);

	btn.disabled = true;

	const sortId = btn.dataset.sort_id;
	const filters = getSelections();

	const limitValue = document.getElementById('NpcLimitSelectId').value;
	const sortValue = document.getElementById(sortId).value;
	const sortOrder = document.getElementById('AscDescSelectId').value;

	const selections = {
		agg: '',
		sort: sortValue,
		group: '',
		limit: limitValue,
		min: '',
		order: sortOrder,
	};

	const obj = {
		selections: selections,
		filters: filters
	};

	const json = JSON.stringify(obj);

	//console.log(json);

	callServer(action, json, function (data) { loadData(data, btn) });
}

function loadData(data, btn) {
	//console.log(data);
	const obj = JSON.parse(data);

	const container = document.getElementById('ResultsDiv');
	//clear out any existing children
	container.innerHTML = '';

	try {
		const dataTable = document.createElement('table');
		dataTable.classList.add('tableBorder', 'w-100', 'dataTable');
		dataTable.id = 'AggTableId';
		container.appendChild(dataTable);

		//Create header row of table using keys from first node
		const dataTableHead = document.createElement('thead');
		dataTable.append(dataTableHead);
		const tableHeader = document.createElement('tr');
		dataTableHead.appendChild(tableHeader);

		Object.keys(obj['data'][0]).forEach(key => {
			const th = document.createElement('th');
			th.textContent = key;
			if (!key.includes('id')) {
				tableHeader.appendChild(th);
			}
		});

		const nameCounts = {};
		for (const val of obj['data']) {
			nameCounts[val.name] = (nameCounts[val.name] || 0) + 1;
		}

		for (let i = 0; i < obj['data'].length; i++) {
			const row = document.createElement('tr');
			dataTable.appendChild(row);
			Object.keys(obj['data'][i]).forEach(key => {
				let _value = obj['data'][i][key];

				const classList = ['dataCell'];

				if (key == 'name') {
					const isDuplicateName = nameCounts[obj['data'][i].name] > 1;
					const id = obj['data'][i]['id'];
					row.append(createTd(buildObjectLinkButton(_value, id, obj.type, 'npcLinkBtn', isDuplicateName), classList));
				} else if (key == 'class') {
					const id = obj['data'][i]['class_id'];
					const btn = buildObjectLinkButton(_value, id, key);
					row.append(createTd(btn, classList));
				} else if (key == 'race') {
					const id = obj['data'][i]['race_id'];
					const btn = buildObjectLinkButton(_value, id, key);
					row.append(createTd(btn, classList));
				} else if (key == 'faction') {
					const id = obj['data'][i]['faction_id'];
					const btn = buildObjectLinkButton(_value, id, key);
					row.append(createTd(btn, classList));
				} else if (!key.includes('id')) {
					row.appendChild(createTd(_value, ['dataCell']));
				}

			});

		}

	} catch (e) {
		const div = document.createElement('div');
		div.textContent = 'No Results Found';
		container.append(div);
	}

	btn.disabled = false;
}

/* Functions in this section render the user interface 
including filter check box lists and select boxes that are used to generate queries*/
function buildInterface(obj) {

	//console.log(JSON.stringify(obj['Filters']));

	//Populate the Select Box options for Run Query function
	generateOptions(obj['GroupBy'], 'GroupBySelectId', 0);
	generateOptions(obj['Agg'], 'AggSelectId', 0);
	generateOptions(obj['Sorts']['SortBy'], 'SortBySelectId', 0);


	//Create select boxes & populate the options for Find Npc function
	generateOptions(obj['Objects'], 'ObjectSelectId', 0);

	generateOptions(obj['Sorts']['ClassSort'], 'ClassSortSelectId', 0);
	generateOptions(obj['Sorts']['ItemSort'], 'ItemSortSelectId', 0);
	generateOptions(obj['Sorts']['ToolSort'], 'ToolSortSelectId', 0);
	generateOptions(obj['Sorts']['ApparatusSort'], 'ApparatusSortSelectId', 0);
	generateOptions(obj['Sorts']['ArmorSort'], 'ArmorSortSelectId', 0)
	generateOptions(obj['Sorts']['CreatureSort'], 'CreatureSortSelectId', 0)
	generateOptions(obj['Sorts']['MagicEffectSort'], 'MagicEffectSortSelectId', 0);
	generateOptions(obj['Sorts']['MiscItemSort'], 'MiscItemSortSelectId', 0)
	generateOptions(obj['Sorts']['WeaponSort'], 'WeaponSortSelectId', 0)
	generateOptions(obj['Sorts']['SpellSort'], 'SpellSortSelectId', 0)
	generateOptions(obj['Sorts']['FactionSort'], 'FactionSortSelectId', 0)
	generateOptions(obj['Sorts']['RaceSort'], 'RaceSortSelectId', 0)

	generateOptions(obj['Sorts']['SortBy'], 'NpcSortSelectId', 1);

	//Create collapsible checkbox filter sections
	generateFilters(obj['Filters']);

}
//create the options for the various select boxes
function generateOptions(objArray, targetElementId, selected) {
	const parent = document.getElementById(targetElementId);

	objArray.forEach((item) => {
		const element = document.createElement('option');
		element.value = item;
		element.text = item;
		parent.appendChild(element);
	});
	parent.selectedIndex = selected;

}

function generateFilters(obj) {

	const parent = document.getElementById('FilterContainerId');
	const collapsibleFilterContainer = document.getElementById('collapsibleFilterContainer');

	Object.keys(obj).forEach(key => {

		const cleanName = key.replace(/\s+/g, '');
		//console.log(key + ' ' + cleanName);
		const header = buildHeader(key);
		parent.appendChild(header);

		const collapsingDiv = document.createElement('div');
		collapsingDiv.id = cleanName + 'CollapseId';
		collapsingDiv.classList.add('collapse', 'filterCollapsible', 'col-auto');

		collapsingDiv.append(createDiv(cleanName, ['fw-bold']));

		collapsibleFilterContainer.appendChild(collapsingDiv);


		const label = document.createElement('label');
		label.textContent = 'All';
		label.htmlFor = cleanName + 'AllCheckBoxId';

		const checkbox = document.createElement('input');
		checkbox.type = 'checkbox';
		checkbox.id = cleanName + 'AllCheckBoxId';
		checkbox.value = 'All';
		checkbox.dataset.group = cleanName + 'CheckBox';
		checkbox.addEventListener('change', function (e) { selectAllCheckBox(e.currentTarget) });
		checkbox.checked = true;
		checkbox.classList.add('filter-check-box');

		const div = document.createElement('div');
		div.append(checkbox, label);
		collapsingDiv.appendChild(div);

		buildCheckBoxes(obj[key], collapsingDiv, cleanName, checkbox.dataset.group);

	});
	buildCollapseAllButton(parent);

}

function buildHeader(name) {

	const cleanName = name.replace(/\s+/g, '');

	const header = document.createElement('div');

	const span1 = document.createElement('span');
	const span2 = document.createElement('span');

	header.id = cleanName + "HeaderId";
	header.classList.add('col-auto', 'btn', 'filterBtn', 'filterBtnCollection');

	header.dataset.section = cleanName;
	header.dataset.toggle_target = cleanName + 'CollapseId';
	header.dataset.is_collapsed = 1;
	header.setAttribute('onclick', 'toggleCollapse(this)');

	span1.textContent = name;
	span2.textContent = '+';
	span2.classList.add('PlusMinusSpan');
	header.append(span1, span2);

	return header;
}

function buildCheckBoxes(obj, container, group, parentKey, class2 = '') {
	//console.log(parentKey);

	if (typeof obj[0] === 'string') {
		for (let i = 0; i < obj.length; i++) {

			const checkboxId = parentKey + obj[i] + class2 + "CheckBoxId";

			const label = document.createElement('label');
			label.textContent = obj[i];
			label.htmlFor = checkboxId;

			const checkbox = document.createElement('input');
			checkbox.type = 'checkbox';
			checkbox.id = checkboxId;
			checkbox.value = obj[i];
			checkbox.classList.add(group, 'filter-check-box', parentKey);
			if (class2 != '') checkbox.classList.add(class2);
			checkbox.checked = true;

			const div = document.createElement('div');
			div.classList.add('d-flex');
			div.classList.add('flex-nowrap');

			div.append(checkbox, label);
			container.appendChild(div);
		}
	} else {
		if (typeof obj === 'object') {
			Object.keys(obj).forEach(key => {
				const div = document.createElement('div');
				div.textContent = key;
				div.classList.add('filterHeading', 'text-center');
				container.appendChild(div);
				const class2 = key.replace(/\s+/g, '') + 'CheckBox';
				buildCheckBoxes(obj[key], container, group, parentKey, class2);
			});
		}
	}

}


/* Reading Filters & Selections */

function getSelections() {

	const RACES = getFilters('.RaceCheckBox');
	const CLASSES = getFilters('.ClassCheckBox');
	const FACTIONS = getFilters('.FactionCheckBox');
	const GENDERS = getFilters('.GenderCheckBox');
	const EXPANSIONS = getFilters('.ExpansionCheckBox');
	const SERVICES = getFilters('.ServicesCheckBox');
	const APPARATUS_TYPE = getFilters('.ApparatusTypeCheckBox');
	const ARMOR_TYPE = getFilters('.ArmorTypeCheckBox');
	const CLOTHING_TYPE = getFilters('.ClothingTypeCheckBox');
	const CREATURE_TYPE = getFilters('.CreatureTypeCheckBox');
	const SPELL_TYPE = getFilters('.SpellTypeCheckBox');
	const WEAPON_TYPE = getFilters('.WeaponTypeCheckBox');
	const MAGIC_SCHOOL = getFilters('.MagicSchoolCheckBox');
	const MAGIC_EFFECT = getFilters('.MagicEffectCheckBox');
	const SKILLS = getFilters('.SkillsCheckBox');
	const ATTRIBUTES = getFilters('.AttributesCheckBox');
	const SPECIALIZATION = getFilters('.SpecializationCheckBox');
	const IS_PLAYABLE = getFilters('.IsPlayableCheckBox');
	const IS_BEAST_RACE = getFilters('.IsBeastRaceCheckBox');

	const LOCATIONS = getFilters('.LocationCheckBox');

	const obj = {
		"RACES": RACES,
		"CLASSES": CLASSES,
		"FACTIONS": FACTIONS,
		"GENDERS": GENDERS,
		"EXPANSIONS": EXPANSIONS,
		"LOCATIONS": LOCATIONS,
		"SERVICES": SERVICES,
		"APPARATUS_TYPE": APPARATUS_TYPE,
		"ARMOR_TYPE": ARMOR_TYPE,
		"CLOTHING_TYPE": CLOTHING_TYPE,
		"CREATURE_TYPE": CREATURE_TYPE,
		"SPELL_TYPE": SPELL_TYPE,
		"WEAPON_TYPE": WEAPON_TYPE,
		"MAGIC_SCHOOL": MAGIC_SCHOOL,
		"MAGIC_EFFECT": MAGIC_EFFECT,
		"SKILLS": SKILLS,
		"ATTRIBUTES": ATTRIBUTES,
		"SPECIALIZATION": SPECIALIZATION,
		"PLAYABLE_FLAG": IS_PLAYABLE,
		"BEAST_RACE_FLAG": IS_BEAST_RACE,
	};

	return obj;
}

function getFilters(identifier) {

	const checkBoxes = document.querySelectorAll(identifier);

	let objArray = [];

	checkBoxes.forEach(checkbox => {

		if (!checkbox.checked) {
			objArray.push(checkbox.value);
		}
	});
	return objArray;
}

/* Event handling functions  */

function changeObjectSelect(s) {

	//get the button element, text changed to reflect object type being queried
	const button = document.getElementById('listNPCsButton');

	//get list of all filter headers elements and hide them
	const filterBtnCollections = document.querySelectorAll('.filterBtnCollection');
	filterBtnCollections.forEach(fbc => fbc.classList.add('d-none'));

	//get list of all filter checkbox elements and disable them
	const filterCheckBoxes = document.querySelectorAll('.filter-check-box');
	filterCheckBoxes.forEach(fcb => fcb.checked = true);
	filterCheckBoxes.forEach(fcb => fcb.disabled = 'true');


	//get list of all sorting select boxes and add hide them
	const sortSelectElements = document.querySelectorAll('.sort-element');
	sortSelectElements.forEach(sse => sse.classList.add('d-none'));

	//Get references to all of the filter elements
	const LocationBoxes = document.querySelectorAll('.LocationCheckBox');
	const LocationAllCheckBox = document.getElementById('LocationAllCheckBoxId');
	const LocationHeader = document.getElementById('LocationHeaderId');

	const ExpansionBoxes = document.querySelectorAll('.ExpansionCheckBox');
	const ExpansionAllCheckBox = document.getElementById('ExpansionAllCheckBoxId');
	const ExpansionHeader = document.getElementById('ExpansionHeaderId');

	const RaceCheckBoxes = document.querySelectorAll('.RaceCheckBox');
	const RaceAllCheckBox = document.getElementById('RaceAllCheckBoxId');
	const RaceHeader = document.getElementById('RaceHeaderId');

	const ClassCheckBoxes = document.querySelectorAll('.ClassCheckBox');
	const ClassAllCheckBox = document.getElementById('ClassAllCheckBoxId');
	const ClassHeader = document.getElementById('ClassHeaderId');

	const FactionCheckBoxes = document.querySelectorAll('.FactionCheckBox');
	const FactionAllCheckBox = document.getElementById('FactionAllCheckBoxId');
	const FactionHeader = document.getElementById('FactionHeaderId');

	const GenderCheckBoxes = document.querySelectorAll('.GenderCheckBox');
	const GenderAllCheckBox = document.getElementById('GenderAllCheckBoxId');
	const GenderHeader = document.getElementById('GenderHeaderId');

	const ServicesCheckBoxes = document.querySelectorAll('.ServicesCheckBox');
	const ServicesAllCheckBox = document.getElementById('ServicesAllCheckBoxId');
	const ServicesHeader = document.getElementById('ServicesHeaderId');

	const ApparatusTypeCheckBoxes = document.querySelectorAll('.ApparatusTypeCheckBox');
	const ApparatusTypeAllCheckBox = document.getElementById('ApparatusTypeAllCheckBoxId');
	const ApparatusTypeHeader = document.getElementById('ApparatusTypeHeaderId');

	const ArmorTypeCheckBoxes = document.querySelectorAll('.ArmorTypeCheckBox');
	const ArmorTypeAllCheckBox = document.getElementById('ArmorTypeAllCheckBoxId');
	const ArmorTypeHeader = document.getElementById('ArmorTypeHeaderId');

	const ClothingTypeCheckBoxes = document.querySelectorAll('.ClothingTypeCheckBox');
	const ClothingTypeAllCheckBox = document.getElementById('ClothingTypeAllCheckBoxId');
	const ClothingTypeHeader = document.getElementById('ClothingTypeHeaderId');

	const CreatureTypeCheckBoxes = document.querySelectorAll('.CreatureTypeCheckBox');
	const CreatureTypeAllCheckBox = document.getElementById('CreatureTypeAllCheckBoxId');
	const CreatureTypeHeader = document.getElementById('CreatureTypeHeaderId');

	const MagicEffectCheckBoxes = document.querySelectorAll('.MagicEffectCheckBox');
	const MagicEffectAllCheckBox = document.getElementById('MagicEffectAllCheckBoxId');
	const MagicEffectHeader = document.getElementById('MagicEffectHeaderId');

	const MagicSchoolCheckBoxes = document.querySelectorAll('.MagicSchoolCheckBox');
	const MagicSchoolAllCheckBox = document.getElementById('MagicSchoolAllCheckBoxId');
	const MagicSchoolHeader = document.getElementById('MagicSchoolHeaderId');

	const SpellTypeCheckBoxes = document.querySelectorAll('.SpellTypeCheckBox');
	const SpellTypeAllCheckBox = document.getElementById('SpellTypeAllCheckBoxId');
	const SpellTypeHeader = document.getElementById('SpellTypeHeaderId');

	const WeaponTypeCheckBoxes = document.querySelectorAll('.WeaponTypeCheckBox');
	const WeaponTypeAllCheckBox = document.getElementById('WeaponTypeAllCheckBoxId');
	const WeaponTypeHeader = document.getElementById('WeaponTypeHeaderId');

	const SkillsCheckBoxes = document.querySelectorAll('.SkillsCheckBox');
	const SkillsAllCheckBox = document.getElementById('SkillsAllCheckBoxId');
	const SkillsHeader = document.getElementById('SkillsHeaderId');

	const AttributesCheckBoxes = document.querySelectorAll('.AttributesCheckBox');
	const AttributesAllCheckBox = document.getElementById('AttributesAllCheckBoxId');
	const AttributesHeader = document.getElementById('AttributesHeaderId');

	const SpecializationCheckBoxes = document.querySelectorAll('.SpecializationCheckBox');
	const SpecializationAllCheckBox = document.getElementById('SpecializationAllCheckBoxId');
	const SpecializationHeader = document.getElementById('SpecializationHeaderId');

	const IsPlayableCheckBoxes = document.querySelectorAll('.IsPlayableCheckBox');
	const IsPlayableAllCheckBox = document.getElementById('IsPlayableAllCheckBoxId');
	const IsPlayableHeader = document.getElementById('IsPlayableHeaderId');

	const IsBeastRaceCheckBoxes = document.querySelectorAll('.IsBeastRaceCheckBox');
	const IsBeastRaceAllCheckBox = document.getElementById('IsBeastRaceAllCheckBoxId');
	const IsBeastRaceHeader = document.getElementById('IsBeastRaceHeaderId');

	//re-enable and unhide the filter elements depending on what object type was selected from the dropdown
	let selectElementId = 'NpcSortSelectId';
	let buttonText = 'Find NPCs';

	switch (s.value) {
		case "npcs":
			buttonText = 'Find NPCs';
			selectElementId = 'NpcSortSelectId';

			LocationBoxes.forEach(cb => cb.disabled = false);
			LocationAllCheckBox.disabled = false;
			LocationHeader.classList.remove('d-none');

			RaceCheckBoxes.forEach(cb => cb.disabled = false);
			RaceAllCheckBox.disabled = false;
			RaceHeader.classList.remove('d-none');

			ClassCheckBoxes.forEach(cb => cb.disabled = false);
			ClassAllCheckBox.disabled = false;
			ClassHeader.classList.remove('d-none');

			FactionCheckBoxes.forEach(cb => cb.disabled = false);
			FactionAllCheckBox.disabled = false;
			FactionHeader.classList.remove('d-none');

			GenderCheckBoxes.forEach(cb => cb.disabled = false);
			GenderAllCheckBox.disabled = false;
			GenderHeader.classList.remove('d-none');

			ServicesCheckBoxes.forEach(cb => cb.disabled = false);
			ServicesAllCheckBox.disabled = false;
			ServicesHeader.classList.remove('d-none');
			break;

		case "alchemy":
			buttonText = 'Find Alchemy';
			selectElementId = 'MiscItemSortSelectId';

			SkillsCheckBoxes.forEach(cb => cb.disabled = false);
			SkillsAllCheckBox.disabled = false;
			SkillsHeader.classList.remove('d-none');

			AttributesCheckBoxes.forEach(cb => cb.disabled = false);
			AttributesAllCheckBox.disabled = false;
			AttributesHeader.classList.remove('d-none');

			MagicEffectCheckBoxes.forEach(cb => cb.disabled = false);
			MagicEffectAllCheckBox.disabled = false;
			MagicEffectHeader.classList.remove('d-none');
			break;

		case "apparatuses":
			buttonText = 'Find Apparatus';
			selectElementId = 'ApparatusSortSelectId';

			ApparatusTypeCheckBoxes.forEach(cb => cb.disabled = false);
			ApparatusTypeAllCheckBox.disabled = false;
			ApparatusTypeHeader.classList.remove('d-none');
			break;

		case "armors":
			buttonText = 'Find Armor';
			selectElementId = 'ArmorSortSelectId';

			ArmorTypeCheckBoxes.forEach(cb => cb.disabled = false);
			ArmorTypeAllCheckBox.disabled = false;
			ArmorTypeHeader.classList.remove('d-none');
			break;

		case "books":
			buttonText = 'Find Books';
			selectElementId = 'ItemSortSelectId';
			break;

		case "classes":
			buttonText = 'Find Classes';
			selectElementId = 'ClassSortSelectId';

			SpecializationCheckBoxes.forEach(cb => cb.disabled = false);
			SpecializationAllCheckBox.disabled = false;
			SpecializationHeader.classList.remove('d-none');

			IsPlayableCheckBoxes.forEach(cb => cb.disabled = false);
			IsPlayableAllCheckBox.disabled = false;
			IsPlayableHeader.classList.remove('d-none');

			SkillsCheckBoxes.forEach(cb => cb.disabled = false);
			SkillsAllCheckBox.disabled = false;
			SkillsHeader.classList.remove('d-none');

			AttributesCheckBoxes.forEach(cb => cb.disabled = false);
			AttributesAllCheckBox.disabled = false;
			AttributesHeader.classList.remove('d-none');
			break;

		case "clothing":
			buttonText = 'Find Clothing';
			selectElementId = 'ItemSortSelectId';

			ClothingTypeCheckBoxes.forEach(cb => cb.disabled = false);
			ClothingTypeAllCheckBox.disabled = false;
			ClothingTypeHeader.classList.remove('d-none');
			break;

		case "creatures":
			buttonText = 'Find Creatures';
			selectElementId = 'CreatureSortSelectId';

			CreatureTypeCheckBoxes.forEach(cb => cb.disabled = false);
			CreatureTypeAllCheckBox.disabled = false;
			CreatureTypeHeader.classList.remove('d-none');
			break;

		case "factions":
			buttonText = 'Find Factions';
			selectElementId = 'FactionSortSelectId';

			AttributesCheckBoxes.forEach(cb => cb.disabled = false);
			AttributesAllCheckBox.disabled = false;
			AttributesHeader.classList.remove('d-none');
			break;

		case "ingredients":
			buttonText = 'Find Ingredients';
			selectElementId = 'MiscItemSortSelectId';

			SkillsCheckBoxes.forEach(cb => cb.disabled = false);
			SkillsAllCheckBox.disabled = false;
			SkillsHeader.classList.remove('d-none');

			AttributesCheckBoxes.forEach(cb => cb.disabled = false);
			AttributesAllCheckBox.disabled = false;
			AttributesHeader.classList.remove('d-none');

			MagicEffectCheckBoxes.forEach(cb => cb.disabled = false);
			MagicEffectAllCheckBox.disabled = false;
			MagicEffectHeader.classList.remove('d-none');
			break;

		case "lockpicks":
			buttonText = 'Find Lockpicks';
			selectElementId = 'ToolSortSelectId';
			break;

		case 'magiceffects':
			buttonText = 'Find Magic Effect';
			selectElementId = 'MagicEffectSortSelectId';

			MagicSchoolCheckBoxes.forEach(cb => cb.disabled = false);
			MagicSchoolAllCheckBox.disabled = false;
			MagicSchoolHeader.classList.remove('d-none');
			break;

		case "miscitems":
			buttonText = 'Find Misc Items';
			selectElementId = 'MiscItemSortSelectId';
			break;

		case "probes":
			buttonText = 'Find Probes';
			selectElementId = 'ToolSortSelectId';
			break;

		case "races":
			buttonText = 'Find Races';
			selectElementId = 'RaceSortSelectId';

			IsPlayableCheckBoxes.forEach(cb => cb.disabled = false);
			IsPlayableAllCheckBox.disabled = false;
			IsPlayableHeader.classList.remove('d-none');

			IsBeastRaceCheckBoxes.forEach(cb => cb.disabled = false);
			IsBeastRaceAllCheckBox.disabled = false;
			IsBeastRaceHeader.classList.remove('d-none');

			break;

		case "repairitems":
			buttonText = 'Find Repair Items';
			selectElementId = 'ToolSortSelectId';
			break;

		case "spells":
			buttonText = 'Find Spell';
			selectElementId = 'SpellSortSelectId';

			SpellTypeCheckBoxes.forEach(cb => cb.disabled = false);
			SpellTypeAllCheckBox.disabled = false;
			SpellTypeHeader.classList.remove('d-none');

			SkillsCheckBoxes.forEach(cb => cb.disabled = false);
			SkillsAllCheckBox.disabled = false;
			SkillsHeader.classList.remove('d-none');

			AttributesCheckBoxes.forEach(cb => cb.disabled = false);
			AttributesAllCheckBox.disabled = false;
			AttributesHeader.classList.remove('d-none');

			MagicEffectCheckBoxes.forEach(cb => cb.disabled = false);
			MagicEffectAllCheckBox.disabled = false;
			MagicEffectHeader.classList.remove('d-none');
			break;

		case "weapons":
			selectElementId = 'WeaponSortSelectId';
			buttonText = 'Find Weapon';

			WeaponTypeCheckBoxes.forEach(cb => cb.disabled = false);
			WeaponTypeAllCheckBox.disabled = false;
			WeaponTypeHeader.classList.remove('d-none');
			break;
	}

	document.getElementById(selectElementId).classList.remove('d-none');
	button.textContent = buttonText;
	button.dataset.sort_id = selectElementId;

	ExpansionBoxes.forEach(cb => cb.disabled = false);
	ExpansionAllCheckBox.disabled = false;
	ExpansionHeader.classList.remove('d-none');

}

function toggleCollapse(h) {

	const state = h.dataset.is_collapsed;
	const targetName = h.dataset.toggle_target;
	const target = document.getElementById(targetName);
	const section = h.dataset.section;
	const span = h.querySelector('.PlusMinusSpan');

	if (state == 1) {
		target.classList.remove('collapse');
		h.dataset.is_collapsed = 0;
		span.textContent = '-';
		h.classList.add('filterBtnBorder');
	} else {
		target.classList.add('collapse');
		h.dataset.is_collapsed = 1;
		span.textContent = '+';
		h.classList.remove('filterBtnBorder');
	}
}

function selectAllCheckBox(c) {

	const group = c.dataset.group;
	const elements = document.getElementsByClassName(group);

	if (c.checked) {
		for (let i = 0; i < elements.length; i++) {
			elements[i].checked = true;
		}
	} else {
		for (let i = 0; i < elements.length; i++) {
			elements[i].checked = false;
		}
	}
}

function buildCollapseAllButton(container) {
	const button = document.createElement('div');
	button.textContent = 'Collapse All';
	button.classList.add('btn', 'col-auto', 'filterBtn');

	container.append(button);

	button.addEventListener('click', function (e) {
		const filterButtons = document.querySelectorAll('.filterBtnCollection');
		for (const filterButton of filterButtons) {

			const targetName = filterButton.dataset.toggle_target;
			const target = document.getElementById(targetName);
			target.classList.add('collapse');

			const span = filterButton.querySelector('.PlusMinusSpan');
			span.textContent = '+';

			filterButton.classList.remove('filterBtnBorder');
			filterButton.dataset.is_collapsed = 1;
		};

	});
}

function debounce(fn, delay = 150) {
	let timer;
	return (...args) => {
		clearTimeout(timer);
		timer = setTimeout(() => fn(...args), delay);
	};
}

/* Post Update Builder & Button functions */
function buildUpdatePosts(data) {

	const container = document.getElementById('postContainer');

	const obj = JSON.parse(data);
	obj.forEach((post, index) => {

		const div = document.createElement('div');
		div.classList.add('p-2', 'postContainer');
		if (index > 0) div.classList.add('d-none');

		const row1 = document.createElement('div');
		row1.classList.add('row');

		const col1 = document.createElement('div');
		col1.classList.add('col-8', 'postTitleCol');
		const col2 = document.createElement('div');
		col2.classList.add('col-4', 'postDateCol');

		row1.append(col1, col2);

		const row2 = document.createElement('div');
		row2.classList.add('row');

		const textCol = document.createElement('div');
		textCol.classList.add('col');

		row2.append(textCol);

		col1.innerHTML = post.post_title;
		col2.textContent = post.post_date;

		const p = document.createElement('p');
		p.innerHTML = post.post_text;
		textCol.append(p);

		div.append(row1, row2);

		container.append(div);

	});
}

function pastUpdatesButtonClick(btn) {

	const btnTop = document.getElementById('pastUpdatesButtonTop');

	if (btn.dataset.state == 'show') {
		document.querySelectorAll('.postContainer').forEach(el => {
			el.classList.remove('d-none');
		});
		btnTop.classList.remove('d-none');
		document.querySelectorAll('.past-updates-btn').forEach(el => {
			el.textContent = "hide past updates";
			el.dataset.state = 'hide';
		});
	} else {
		btnTop.classList.add('d-none');
		document.querySelectorAll('.postContainer').forEach((el, index) => {
			if (index != 0) el.classList.add('d-none');
		});

		document.querySelectorAll('.past-updates-btn').forEach(el => {
			el.textContent = 'show past updates';
			el.dataset.state = 'show';
		});

	}
}

function buildTopList(data) {
	//console.log(data);
	const table = document.getElementById('TopListTableId');
	if (table) {
		const topListObject = JSON.parse(data);

		topListObject.forEach(row => {
			const tr = document.createElement('tr');

			const tdLink = document.createElement('td');
			const button = buildObjectLinkButton(row.name, row.Id, row.Type)
			tdLink.append(button);

			const tdType = document.createElement('td');
			tdType.textContent = row.Type;

			const tdViews = document.createElement('td');
			tdViews.textContent = row.Views;

			tr.append(tdLink, tdType, tdViews);
			table.append(tr);
		});
	}
}

function buildHeaderTable(table, data) {

	data.forEach(row => {
		const tr = document.createElement('tr');

		const tdAuthor = document.createElement('td');
		tdAuthor.style.color = '#6F541D';
		tdAuthor.textContent = row.author;

		const tdDescription = document.createElement('td');
		tdDescription.textContent = row.description;

		tr.append(tdAuthor, tdDescription);
		table.append(tr);
	});
}