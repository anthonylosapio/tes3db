async function callServer(url, callback, source = null) {
	//console.log('action:'+action+' data:'+data);
	if (source) {
		toggleLoadingIndicator(source);
	}

	try {
		const response = await fetch(url, {
			method: "GET",
			headers: {
				"Content-Type": "application/x-www-form-urlencoded"
			}
		});

		const text = await response.text();

		if (response.ok) {
			//console.log(text);
			callback(text);
		} else {
			console.log(text);
		}
	} catch (err) {
		console.log(err);
	} finally {
		if (source) {
			toggleLoadingIndicator(source);
		}
	}
}

function buildObjectLinkButton(objName, objId, objType, cssClass = 'npcLinkBtn', isDuplicateName = false, includeTypePrefix = false) {

	let page = 'npc.html';

	switch (objType) {
		case 'npc':
			page = 'npc.html';
			break;
		case 'alchemy':
			page = 'alchemy.html';
			break;
		case 'apparatus':
			page = 'apparatus.html';
			break;
		case 'armor':
			page = 'armor.html';
			break;
		case 'birthsign':
			page = 'birthsign.html';
			break;
		case 'book':
			page = 'book.html';
			break;
		case 'cell':
			page = 'cell.html';
			break;
		case 'class':
			page = 'class.html';
			break;
		case 'clothing':
			page = 'clothing.html';
			break;
		case 'container':
			page = 'container.html';
			break;
		case 'creature':
			page = 'creature.html';
			break;
		case 'faction':
			page = 'faction.html';
			break;
		case 'ingredient':
			page = 'ingredient.html';
			break;
		case 'leveleditem':
			page = 'leveleditem.html';
			break;
		case 'lockpick':
			page = 'lockpick.html';
			break;
		case 'magiceffect':
			page = 'magiceffect.html';
			break;
		case 'miscitem':
			page = 'miscitem.html';
			break;
		case 'probe':
			page = 'probe.html';
			break;
		case 'race':
			page = 'race.html';
			break;
		case 'repairitem':
			page = 'repairitem.html';
			break;
		case 'skill':
			page = 'skill.html';
			break;
		case 'spell':
			page = 'spell.html';
			break;
		case 'topic':
			page = 'topic.html';
			break;
		case 'weapon':
			page = 'weapon.html';
			break;

	}

	const row = createDiv('', ['row', 'g-0', 'align-items-end']);
	if (cssClass) row.classList.add(cssClass);

	//Used to include the type of the object, appears before the name
	if (includeTypePrefix) {
		const span = document.createElement('span');
		span.textContent = objType;
		span.classList.add('search-result-type');
		row.append(createDiv(span, ['col-auto', 'me-1']));
	}

	row.append(createDiv(objName, ['col-auto', 'text-decoration-underline', 'me-1']));
	if (isDuplicateName) row.append(createDiv(objId, ['table-label', 'small', 'col-auto', 'text-decoration-none', 'text-left']));

	const spinnerWrap = document.createElement('div');
	spinnerWrap.classList.add(
		'button-spinner-span',
		'd-none',
		'position-absolute',
		'top-50',
		'start-50',
		'translate-middle',
		'opacity-50'
	);

	const spinner = document.createElement('div');
	spinner.classList.add('spinner-border');

	spinnerWrap.appendChild(spinner);

	const button = document.createElement('a');
	button.title = objId;
	button.classList.add('position-relative', 'no-style');
	button.append(row, spinnerWrap);

	const url = `${page}?id=${objId}`;
	button.href = url;
	button.target = '_blank';
	return button;
}

function toggleLoadingIndicator(btn) {
	btn.toggleAttribute('disabled');
	const spinner = btn.querySelector('.button-spinner-span');
	spinner.classList.toggle('d-none');
	spinner.classList.toggle('d-flex');
}

async function loadSeachObject() {

	const overlay = document.getElementById('searchOverlay');
	const start = performance.now();
	const spinner = buildSpinner();
	overlay.append(spinner);
	overlay.style.display = 'flex';

	callServer("/api/search", function (rawData) {
		const obj = JSON.parse(rawData);
		obj.forEach(obj => trie.insert(obj.name.toLowerCase(), obj));
		const end = performance.now();
		console.log(`Elapsed: ${end - start} ms`);
		overlay.style.display = 'none';
	});
}

function buildSpinner() {
	const container = document.createElement('div');
	container.classList.add('content', 'content-border', 'd-flex', 'justify-content-center', 'align-items-center', 'w-100', 'h-100');
	container.classList.add('spinner-container');
	const spinner = document.createElement('span');
	spinner.classList.add('spinner-border');
	container.append(spinner);
	return container;
}

function isNullOrEmpty(value) {
	return !value;
}

function isString(value) {
	return typeof value === 'string';
}

function isDOMElement(value) {
	return value instanceof Element;
}

function createDiv(content, classList = null) {
	const div = document.createElement('div');

	if (isString(content) || typeof (content) === 'number') div.textContent = content;
	if (isDOMElement(content)) div.append(content);
	if (classList) {
		for (const c of classList) {
			div.classList.add(c);
		}
	}
	return div;
}

function createTd(content, classList = null, colspan = 1) {
	const td = document.createElement('td');
	td.colSpan = colspan;
	if (isString(content) || typeof (content) === 'number') td.textContent = content;
	if (isDOMElement(content)) td.append(content);
	if (classList) {
		for (const c of classList) {
			td.classList.add(c);
		}
	}

	return td;
}

function createSpan(content, classList = null) {
	const span = document.createElement('span');

	if (isString(content) || typeof (content) === 'number') span.textContent = content;
	if (isDOMElement(content)) span.append(content);
	if (classList) {
		for (const c of classList) {
			span.classList.add(c);
		}
	}
	return span;
}