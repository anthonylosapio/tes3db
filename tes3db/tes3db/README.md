# tes3db

Run your own local wiki automatically built of off your custom Morrowind mod list.


tes3db converts Morrowind json files to a searchable/query-able SQLite database served through an ASP.NET Core minimal API & with a web based front end (with options to export to csv/tsv or MySQL/Postres import). 

This tool currently requires you to first use [tes3conv](https://github.com/Greatness7/tes3conv) to convert .esm files to json.


## Usage

When run, tes3db.exe checks its base directory for JSON files, extracts game objects, loads them into a SQLite database, then launches a browser based UI to search and query the data.

### steps
Place json files in the same directory as tes3db.exe.

The file name will be used to populate the Expansion column of the output. For example, if the file is named "Morrowind.json", the Expansion column will be populated with "Morrowind".

If converting multiple json files at once, prefix the file name with a number so that they will be processed in the correct order. 
Example: 01Morrowind.json, 02Tribunal.json, 03Bloodmoon.json, 04Tamriel Rebuilt.json. This will ensure NPC's are assigned the correct
expansion as some NPC's appear in multiple .esm files. The leading digits will be stripped away before the expansion name is assigned so 01Morrowind.json will still become "Morrowind".


#### npc.json
NPC's in the game with the AutoCalculate flag will not have attributes or skills in the json file created after conversion using tes3conv. To retrieve this you can open up the .esp/.esm file in the construction set and toggle AutoCalc off. Then save the results to npc.esm, and convert that to json using tes3conv.
tes3db will look for a npc.json file and handle it accordingly so that Expansion is still properly assigned, but the NPC's also have their Attributes and skills recorded.


### Options

```
tes3db.exe [options]
```

| Option                 | Description                                                                      |
|------------------------|----------------------------------------------------------------------------------|
| `--refresh, -r`        | Force refresh of sqlite database                                                 |
| `--format, -f <type>`  | Output type: sqlite, csv, tsv, mysql or postgres (default: sqlite)               |
| `--no-headers`         | Exclude column headers (csv/tsv only)                                            |
| `--skip`               | Skip NPCs missing Cell, Region, Attribute or Skill properties                    |
| `--verbose`            | Display verbose output                                                           |
| `--no-serve`           | Do not start the local read-only REST API server                                 |
| `--no-browser`         | Do not open a web browser when starting the API server                           |
| `--prefix, -p <prefix>`| Prefix for output files, helpful if keeping expansions separate (default: none)  |
| `--help`               | Show this help message                


## Site

You can see an extended version of web interface in action at [tes3db.com](https://tes3db.com).