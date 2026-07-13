# tes3db

Command line tool to convert Morrwond json files to database friendly formats (sql or csv/tsv). 
This tool requires you to first use [tes3conv](https://github.com/Greatness7/tes3conv) to convert .esm files to json.


## Usage
Place json files in the same directory as tes3db.exe.

The file name will be used to populate the Expansion column of the output. For example, if the file is named "Morrowind.json", the Expansion column will be populated with "Morrowind".

If converting multiple json files at once, prefix the file name with a number so that they will be processed in the correct order. 
Example: 01Morrowind.json, 02Tribunal.json, 03Bloodmoon.json, 04Tamriel Rebuilt.json. This will ensure NPC's are assigned the correct
expansion as some NPC's appear in multiple .esm files. The leading digits will be stripped away before the expansion name is assigned so 01Morrowind.json will still become "Morrowind".


### npc.json
NPC's in the game with the AutoCalculate flag will not have attributes or skills in the json file created after conversion using tes3conv. To retrieve this you can open up the .esp/.esm file in the construction set and toggle AutoCalc off. Then save the results to npc.esm, and convert that to json using tes3conv.
tes3db will look for a npc.json file and handle it accordingly so that Expansion is still properly assigned, but the NPC's also have their Attributes and skills recorded.


### Options
tes3db.exe [options]
|||
|-------------------------|----------------------------------------------------------------------------------|
|`--format, -f <format>`  |  Output format: csv, tsv, mysql or postgres (default: csv)						 |
|`--prefix, -p <type>`    |  Prefix for output files, helpful if keeping expansions separate (default: none)"|
|`--npc <name>`           |  db Table & output File name for extracted NPCs (default: npc)					 |
|`--no-headers`           |  Exclude column headers in (csv/tsv only)										 |
|`--skip`                 |  Will skip NPCs missing Cell,Region,Attribute or Skill poperties				 |
|`--verbose`              |  Show verbose outut in console													 |
|`--help -help`           |  Show this help message															 |


## sql

The [sql section](tes3db/sql) contains sql scripts with CREATE TABLE statements for both MySQL and PostgreSQL that match the output so it can be imported easily. There is also a cleanup script that can be use to normalize some of the fields to make them more human readable.

## Site

The [site section](tes3db/site) contains php, js, and css files that can be used to analyze the data once it has been loaded into a databse. 
You can see it in action at [tes3db.com](https://tes3db.com).
