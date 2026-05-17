# tes3db

Command line tool to convert Morrwond json files to database friendly formats (sql or csv). 
This tool requires you to first use tes3conv to convert .esp & .esm files to json.


## Usage
Place json files in the same directory as tes3db.exe.

The file name will be used to populate the Expansion column of the output. For example, if the file is named "Morrowind.json", the Expansion column will be populated with "Morrowind".

If converting multiple json files at once, prefix the file name with a number so that they will be processed in the correct order. 
Example: 01Morrowind.json, 02Tribunal.json, 03Bloodmoon.json, 04Tamriel Rebuilt.json. This will ensure NPC's are assigned the correct
expansion as some NPC's appear in multiple .esm files. The leading digits will be stripped away before the expansion name is assigned.


### npc.json
NPC's in the game with the AutoCalculate flag will not have attributes or skills in the json file. To retrieve this you can open up the .esp/.esm file in the construction set and toggle AutoCalc off. Then save the results to npc.esm, and convert that to json using tes3conv.
tes3db will look for a npc.json file and handle it accordingly so that Expansion is still properly assigned, but the NPC's also have their Attributes and skills recorded.


### Options
tes3db.exe [options]
|||
|-----------------------|-------------------------------------------------------------------|
|--type, -t <type>      |  Output type: csv or sql (default: csv)                           |
|--sql-type, -s <type>  |  SQL type: mysql or postgresql (default: postgresql)              |
|--npc <name>           |  db Table & output File name for extracted NPCs (default: npc)    |
|--no-headers           |  Exclude column headers in (CSV only)                             |
|--no-skip              |  Wont's skip NPCs missing Cell,Region,Attribute or Skill poperties|
|--help                 |  Show this help message                                           |
