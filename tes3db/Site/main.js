function start(){

	callServer("Home", "", function(data){

		buildInterface(data);

	});	

	//assign functions
	const getDataButton = document.getElementById('getDataButton');
	if (getDataButton) getDataButton.addEventListener('click', () => fetchData(getDataButton) );

	const listNPCsButton = document.getElementById('listNPCsButton');
	if (listNPCsButton) listNPCsButton.addEventListener('click', () => fetchNPCList(listNPCsButton) );
}

function callServer(action, data, callback){

	const params = "action="+action+"&data="+data;

	var xhr = new XMLHttpRequest();

	xhr.open("POST", "controller.php", true);

	xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded");

	xhr.onload = function() {

		if (this.status == 200) {

			callback(this.response);

		};
		if (this.status == 400) {
			console.log(this.response);
		}

	};

	xhr.send(params);

}

function fetchData(btn){
	
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
		min: minValue
	};

	const obj = {
		selections: selections,
		filters: filters
	};
	
	const json = JSON.stringify(obj);
	
	callServer("Query", json, function(data){loadData(data, btn)});
}

function fetchNPCList(btn){
	const container = document.getElementById('ResultsDiv');
	container.innerHTML = '';
	const spinner = buildSpinner();
	container.append(spinner);
	
	btn.disabled = true;
	
	const filters = getSelections();
	
	const limitValue = document.getElementById('NpcLimitSelectId').value;
	const sortValue = document.getElementById('NpcSortSelectId').value;
	
	const selections = {
		agg: '',
		sort: sortValue,
		group: '',
		limit: limitValue,
		min: ''
	};

	const obj = {
		selections: selections,
		filters: filters
	};
	
	const json = JSON.stringify(obj);
	
	callServer("List", json, function(data){loadData(data, btn)});
}

function loadData(data, btn){
	console.log("returned data: "+data);
	const obj = JSON.parse(data);
	const container = document.getElementById('ResultsDiv');	
	//clear out any existing children
	container.innerHTML = '';
		
	try{
		const dataTable = document.createElement('table');
		dataTable.classList.add('tableBorder', 'w-100');
		dataTable.classList.add('dataTable');
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
			if (key!='Id'){
				tableHeader.appendChild(th);
			}			
		});
		
		for(let i = 0; i < obj['data'].length; i++){
			const row = document.createElement('tr');
			dataTable.appendChild(row);
			Object.keys(obj['data'][i]).forEach(key => {
				let _value = obj['data'][i][key];
				
				const cell = document.createElement('td');
				cell.classList.add('dataCell');
				
				if(key=='Name'){
					const id = obj['data'][i]['Id'];
					const btn = buildNpcLinkButton(_value, id);
					cell.append(btn);
					row.appendChild(cell);
				}else if (key!='Id'){
					cell.textContent = _value;
					row.appendChild(cell);
				}
				
				
				
			});
			
		}		
		
	}catch(e){
		const div = document.createElement('div');
		div.textContent = 'No Results Found';
		container.append(div);
	}
		
	btn.disabled = false;
}

function buildNpcLinkButton(npcName, npcId){
	const button = document.createElement('button');
	button.dataset.npcId = npcId;
	button.textContent = npcName;
	button.classList.add('npcLinkBtn');
	button.addEventListener('click', () => callServer("NPC", npcId, function(data){loadNpc(data)})); 
	return button;
}

function loadNpc(data){
	console.log(' LOAD NPC '+data);
	localStorage.setItem('npcData', data);
	window.open('npc.php', '_blank');
}

/* Functions in this section render the user interface */
function buildInterface(jsonString){
	
	const obj = JSON.parse(jsonString);
	
	//Create Group By Drop Down Box
	generateOptions(obj['Group By'], 'GroupBySelectId', 0);
	//Create Drop Down Box
	generateOptions(obj['Sort By'], 'SortBySelectId', 0);
	//Create Drop Down Box
	generateOptions(obj['Agg'], 'AggSelectId', 0);
	
	//Create NPC Sort By Drop Down Box
	generateOptions(obj['Sort By'], 'NpcSortSelectId', 1);

	//Create collapsible checkbox filter sections
	generateFilters(obj['Filters']);
	
}

function generateOptions(objArray, targetElementId, selected){
	const parent = document.getElementById(targetElementId);
	
	objArray.forEach((item, index) => {
		const element = document.createElement('option');
		element.value = item;
		element.text = item;
		parent.appendChild(element);
	});
	parent.selectedIndex = selected;
  
}

function generateFilters(obj){

	const parent = document.getElementById('FilterContainerId');
	const collapsibleFilterContainer = document.getElementById('collapsibleFilterContainer');
	
	Object.keys(obj).forEach(key => {

		const cleanName = key.replace(/\s+/g, '');
		const header = buildHeader(key);
		parent.appendChild(header);
		
		const collapsingDiv = document.createElement('div');
		collapsingDiv.id = cleanName + 'CollapseId';
		collapsingDiv.classList.add('collapse');
		collapsingDiv.classList.add('filterCollapsible');
		collapsingDiv.classList.add('col-auto');

		const cDivHeader = document.createElement('div');
		cDivHeader.textContent = cleanName;
		cDivHeader.classList.add('fw-bold');
		collapsingDiv.append(cDivHeader);
		
		collapsibleFilterContainer.appendChild(collapsingDiv);
		
		
		const label = document.createElement('label');
		label.textContent = 'All';
		label.htmlFor = cleanName + 'AllCheckBoxId';

		const checkbox = document.createElement('input');
		checkbox.type = 'checkbox';
		checkbox.id = cleanName + 'AllCheckBoxId';
		checkbox.value = 'All';
		checkbox.dataset.group = cleanName + 'CheckBox';
		checkbox.addEventListener('change', function(e) {selectAllCheckBox(e.currentTarget)});
		checkbox.checked = true;
		
		const div = document.createElement('div');
		div.append(checkbox, label);
		collapsingDiv.appendChild(div);
		
		buildCheckBoxes(obj[key], collapsingDiv, checkbox.dataset.group);
		
	});
	buildCollapseAllButton(parent);
	
}

function buildHeader(name){

	const cleanName = name.replace(/\s+/g, '');

	const header = document.createElement('div');

	const span1 = document.createElement('span');
	const span2 = document.createElement('span');

	header.id = cleanName + "HeaderId";
	header.classList.add('col-auto','btn','filterBtn','filterBtnCollection');
	
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

function buildCheckBoxes(obj, container, group , class2=''){
		
	if(typeof obj[1] === 'string'){
		for(let i=0; i< obj.length; i++){
			
			const checkboxId = obj[i] + class2 +"CheckBoxId";
			
			const label = document.createElement('label');
			label.textContent = obj[i];
			label.htmlFor = checkboxId;

			const checkbox = document.createElement('input');
			checkbox.type = 'checkbox';
			checkbox.id = checkboxId;
			checkbox.value = obj[i];
			checkbox.classList.add(group);
			if(class2!='') checkbox.classList.add(class2);
			checkbox.checked = true;
			
			const div = document.createElement('div');
			div.classList.add('d-flex');
			div.classList.add('flex-nowrap');
			
			div.append(checkbox, label);
			container.appendChild(div);
		}
	}else{
		if(typeof obj === 'object'){
			Object.keys(obj).forEach(key => { 
				const div = document.createElement('div');
				div.textContent = key;
				div.classList.add('filterHeading');
				container.appendChild(div);
				const class2 = key.replace(/\s+/g, '')+'CheckBox';
				buildCheckBoxes(obj[key], container, group, class2);
			});
		}
	}
	
}	
/* Reading Filters & Selections */

function getSelections(){

	const RACES = getFilters('.RaceCheckBox');
	const CLASSES = getFilters('.ClassCheckBox');
	const FACTIONS = getFilters('.FactionCheckBox');
	const GENDERS = getFilters('.GenderCheckBox');
	const EXPANSIONS = getFilters('.ExpansionCheckBox');
	const SERVICES = getFilters('.ServicesCheckBox');

	const BASE_LOCATIONS = getFilters('.MorrowindCheckBox');
	const BLOODMOON_LOCATIONS = getFilters('.BloodmoonCheckBox');
	const TRIBUNAL_LOCATIONS = getFilters('.TribunalCheckBox');
	const TAMRIEL_LOCATIONS = getFilters('.TamrielRebuiltCheckBox');
	
	let LOCATIONS = {
		BASE_LOCATIONS : BASE_LOCATIONS,
		BLOODMOON_LOCATIONS : BLOODMOON_LOCATIONS,
		TRIBUNAL_LOCATIONS : TRIBUNAL_LOCATIONS,
		TAMRIEL_LOCATIONS : TAMRIEL_LOCATIONS
	};
	const obj = {
		"RACES" : RACES,
		"CLASSES": CLASSES,
		"FACTIONS" : FACTIONS,
		"GENDERS" : GENDERS,
		"EXPANSIONS" : EXPANSIONS,
		"LOCATIONS" : LOCATIONS,
		"SERVICES" : SERVICES,
	};
	
	return obj;
}

function getFilters(identifier){
	
	const checkBoxes = document.querySelectorAll(identifier);
	
	let objArray = [];
	
	checkBoxes.forEach(checkbox => {
		
		if(!checkbox.checked){
			objArray.push(checkbox.value);
		}
	});
	return objArray;
}

/* Event handling functions  */

function toggleCollapse(h){

	const state = h.dataset.is_collapsed;
	const targetName = h.dataset.toggle_target;
	const target = document.getElementById(targetName);
	const section = h.dataset.section;
	const span = h.querySelector('.PlusMinusSpan');

	if(state==1){
		target.classList.remove('collapse');
		h.dataset.is_collapsed = 0;
		span.textContent = '-';
		h.classList.add('filterBtnBorder');
	}else{
		target.classList.add('collapse');
		h.dataset.is_collapsed = 1;
		span.textContent = '+';
		h.classList.remove('filterBtnBorder');
	}
}

function selectAllCheckBox(c){

	const group = c.dataset.group;
	const elements = document.getElementsByClassName(group);

	if(c.checked){
		for(let i=0; i<elements.length; i++){
			elements[i].checked = true;
		}
	}else{
		for(let i=0; i<elements.length; i++){
			elements[i].checked = false;
		}
	}
}

function buildSpinner(){
	const container = document.createElement('div');
	container.classList.add('content');
	container.classList.add('content-border');
	container.classList.add('text-center');
	container.classList.add('py-5');
	const spinner = document.createElement('span');
	spinner.classList.add('spinner-border');
	container.append(spinner);
	return container;
}

function buildCollapseAllButton(container){
	const button = document.createElement('div');
	button.textContent = 'Collapse All';
	button.classList.add('btn', 'col-auto', 'filterBtn');
	
	container.append(button);
	
	button.addEventListener('click', function(e) {
		const filterButtons = document.querySelectorAll('.filterBtnCollection');
		for(const filterButton of filterButtons){
			
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