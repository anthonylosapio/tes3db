function start() {

	const data = localStorage.getItem('npcData');

	if (data != null) {
		npcData = JSON.parse(localStorage.getItem('npcData'));
		renderPage(npcData.data[0]);

		document.getElementById('NpcNameForCountId').textContent = npcData.data[0].Name;
		document.getElementById('NpcCountId').textContent = npcData.count;
	}
	
}

function renderPage(npcData) {
	

	//console.log(npcData);
	
	for (const [key, value] of Object.entries(npcData)) {
		//console.log(key, value);
		if (key === 'Inventory') {
			renderTable(value, 'InventoryTableId');
		} else if (key === 'Spells') {
			renderSpells(value, 'SpellsTableId');
		} else if (key.includes('OFFERS') || key.includes('BARTERS')) {
			renderServices(key, value, 'ServicesTableId');
		}
		else {
			renderProperty(key, value);
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

function renderTable(data, parentId) {
	const obj = JSON.parse(data);
	const tbody = document.getElementById(parentId);
	tbody.innerHTML = '';

	for (const item of obj) {
		const row = document.createElement('tr');
		const itemCell = document.createElement('td');
		itemCell.textContent = item.ItemId;
		const quantityCell = document.createElement('td');
		quantityCell.textContent = item.Quantity;
		row.appendChild(itemCell);
		row.appendChild(quantityCell);
		tbody.appendChild(row);
	}
}

function renderSpells(data, parentId) {
	const obj = JSON.parse(data);
	const tbody = document.getElementById(parentId);
	tbody.innerHTML = '';

	for (const item of obj) {
		const row = document.createElement('tr');
		const spellCell = document.createElement('td');
		spellCell.textContent = item;
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

function renderServices(k, v, parentId) {

	const tbody = document.getElementById(parentId);
	
	if (v === 'True') {
		
		const row = document.createElement('tr');
		const itemCell = document.createElement('td');
		itemCell.textContent = k;
		row.appendChild(itemCell);
		tbody.appendChild(row);
	}
} 