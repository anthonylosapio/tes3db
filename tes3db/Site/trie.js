class TrieNode {
	constructor() {
		this.children = {};
		this.items = []; // store objects that end at this node
	}
}

class Trie {
	constructor() {
		this.root = new TrieNode();
	}
	
	insert(name, obj) {
		let node = this.root;
		for (const char of name) {
			if (!node.children[char]) node.children[char] = new TrieNode();
			node = node.children[char];
		}
		node.items.push(obj); // store the full object
	}

	search(prefix, limit = 10) {
		let node = this.root;
		for (const char of prefix) {
			if (!node.children[char]) return [];
			node = node.children[char];
		}
		return this.collect(node, limit);
	}

	collect(node, limit, results = []) {
		if (results.length >= limit) return results;

		// Add objects stored at this node
		for (const item of node.items) {
			if (results.length >= limit) break;
			results.push(item);
		}
		// Traverse children
		for (const char in node.children) {
			if (results.length >= limit) break;
			this.collect(node.children[char], limit, results);
		}
		return results;
	}
} 