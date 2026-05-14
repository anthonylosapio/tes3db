<!DOCTYPE html>

<html lang="en">

	<head>
		<meta http-equiv=“Pragma” content=”no-cache”>
		<meta http-equiv=“Expires” content=”-1″>
		<meta http-equiv=“CACHE-CONTROL” content=”NO-CACHE”>
		<title>The Morrowind NPC Database</title>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<link href="bootstrap.css" rel="stylesheet">
		<script src="main.js?<?php echo time(); ?>"></script>
		<script>
			window.onload = function() {
				start();
			};
		</script>
		<style>
			.content{
				//background-color: #dcc081;
				background-color: black;
				color: #dcc081;
			}
			.content-border{
				border: solid 2px #6F541D;
			}
			body{
				//background-image: url("img/bg.avif");
				font-family: monospace;
				scrollbar-color: #dcc081 black;
			}
			div{
				scrollbar-width: thin;
			}
			a:link {
				color: purple;
				text-decoration: none;
			}
			a:visited {
				color: #7123DE;
			}
			a:hover {
				color: orange;
				text-decoration: underline;
			}
			a:active {
				color: red;
			}

			input[type="checkbox"] {
				accent-color: purple;
				margin-right: 3px;
				white-space: nowrap;
			}

			.PlusMinusSpan{
				font-weight: 800;
				font-size: 1.1rem;
			}

			.btn:hover {
				outline: 1px solid orange;
			}

			.filterBtn{
				text-align: left;
				padding-top: 0px;
				padding-bottom: 0px;
				font-weight: bolder;
				color: #c7c7c7;
				display: block;
			}
			.filterBtnBorder{
				border: solid orange 1px;	
			}
						
			#ResultsDiv{
				overflow-x: auto; 
				overflow-y: auto; 
				max-height: 90vh;
			}

			.dataTable{
				margin-left: auto;
				margin-right: auto
				border-collapse: collapse;
				width: 98%;
			}
			.dataCell{
				border-bottom: 1px solid #A9A79E;
				border-left: 1px solid #A9A79E;
			}
			thead th {
				position: sticky;
				top: 0;
				background-color: #dcc081;
				color: black;
				z-index: 10;
			}
			.npcLinkBtn{
				background-color: inherit;
				color: inherit;
				border: none;
				text-decoration: underline;
			}
			.npcLinkBtn:hover {
				outline: 1px solid orange;
			}
			
			
			.filterHeading{
				font-weight: bold;
			}
			.filterCollapsible{
				max-height: 60vh;
				overflow-y: auto;
				border-right: 1px solid #A9A79E;
				
			}
			select{
				color: #c7c7c7;
				background-color: black;
				margins: 1px 3px 1px 3px;
			}
		</style>
 	</head>
	<body style="background-color: #2B1A08;">
		<div class="container">
		<!-- BEGIN HEADER ROW -->
			<div class="row g-1 my-1">
				<div class="col-12 content content-border p-2">
					<div class="row">
						<div class="col-auto" style="color: #6F541D">
							<h1>tes3db</h1>
						</div>
						<div class="col-auto">
							<h3>the Morrowind npc database</h3>
						</div>				
					</div>
				</div>
			</div>
		<!-- END HEADER ROW -->
		<!-- BEGIN MENU ROW -->
			<div class="row g-1 my-1" id="menuRow">
				<div class="col content-border content">
					<div class="row justify-content-center align-items-center gx-1">
						<div class="col-auto fw-bold">Show</div>
						<div class="col-auto" style=""><select id="GroupBySelectId"></select></div>
						
						<div class="col-auto fw-bold">by</div>
						<div class="col-auto"><select id="AggSelectId"></select></div>
						<div class="col-auto"  style=""><select id="SortBySelectId" ></select></div>
						<div class="col-auto fw-bold">having at least</div>
						<div class="col-auto">
							<select id="minNpcSelectId">
								<option value="1">1</option>
								<option value="5">5</option>
								<option value="10">10</option>
								<option value="25">25</option>
								<option value="50">50</option>
								<option value="100">100</option>
							</select></div>
						<div class="col-auto fw-bold">NPC(s)</div>
						<div class="col-sm-auto">
							<button class="btn border w-100 position-relative" style="background-color: #dbac64; font-weight: bold; font-size: 1.1em; margin-top: 2px;" id="getDataButton"> 
								<span class="btn-text">Run Query</span>						
							</button>
						</div>	
					</div>
					<div class="row justify-content-center align-items-center">
						<div class="col-auto fw-bold"> - OR - </div>
					</div>
					<div class="row justify-content-center align-items-center gx-1">
						<div class="col-auto fw-bold">List Top </div>
						<div class="col-auto">
							<select id="NpcLimitSelectId">
								<option value="10">10</option>
								<option value="25">25</option>
								<option value="50">50</option>
								<option value="100">100</option>
								<option value="200">200</option>
							</select>
						</div>
						<div class="col-auto fw-bold"> NPCs Sorted By </div>
						<div class="col-auto" style=""><select id="NpcSortSelectId"></select></div>
						<div class="col-sm-auto">
							<button class="btn border w-100 position-relative" style="background-color: #dbac64; font-weight: bold; font-size: 1.1em; margin-top: 2px;" id="listNPCsButton"> 
								<span class="btn-text">Find NPCs</span>
							</button>
						</div>
					</div>
					
					<div class="row justify-content-center align-items-center" id="FilterContainerId">
						<div class="col-auto fw-bold">
							Filters
						</div>
					</div>
					<div class="row justify-content-center" id="collapsibleFilterContainer">
					</div>
				</div>
			</div>
		<!-- END MENU ROW -->
		<!-- BEGIN RESULTS ROW -->
			<div class="row g-0 my-1">
				<div class="col px-0" >
					<div class="content content-border" id="ResultsDiv">
						<div>
							How to use this site.
							<ul>
								<li>Run Query</li>
								<ul>
									<li>Select how you want the data to be aggregated; Average, Sum, Min, Max, and Count (Count is a special case where it will always</li>
									<li>Choose which data point you want the aggregation applied to (Level, Health, Intelligence, Long Blade, etc)</li>
									<li>Select how you want the NPCs to be grouped (Class, Faction, Race, Gender, Expansion)</li>
									<li>Expand the filters and un-check anything you want to exclude from the results (for example, if you only want to include NPCs from Tamriel Rebuilt, expand the "Expansion" section and un-select Morrowind, Tribunal & Bloodmoon)</li>
									<li>Click the Run Query button</li>								
								</ul>
								<li>Find NPCs</li>
								<ul>
									<li>Select How many NPC's you want to include in the results</li>
									<li>Choose which data point you want the NPC's sorted by</li>
									<li>Expand the filters and un-check anything you want to exclude from the results</li>
									<li>Click the Run Query button</li>			
								</ul>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<!-- END RESULTS ROW -->
		<!-- BEGIN CONTENT ROW -->
			<div class="row gx-1 my-1">
				<div class="col">
					<div class="content content-border">
						<p>
						
						</p>
						<p>Notes about the data:<br>
						<ul>
							<li>Only NPCs that are placed in a cell are included here. NPCs added by scripts may not appear in the data.</li>
							<li>Classes are combined with their service offering equivalent by removing the word "serivce" from the end of class names that contained it (eg. "Wise Woman Service" becomes just "Wise Woman", "Smith Service" => "Smith" etc.).</li>
							<li>Some names, races, & factions from Tamriel Rebuilt are altered to be more human readable ("T_Glb_Jeweler"=>"Jeweler", "TR_Fact_SyvvitTong"=>"Syvvit Tong")</li>
							<li>All Khajiit racial variations were converted to "Khajiit" for now, at some point I will update the data to bring those back in.</li>
							<li>All alterations to the raw data can be seen in the SQL file here.</li>
							<li><a href="https://github.com/Greatness7/tes3conv">tes3conv</a> was used to convert the game files to json so that NPC's could be extracted.</li>
						</ul>
						</p>
					</div>
				</div>
			</div>
			
			<div class="row gx-0 my-1 text-center">
				<div class="col">
					<div class="content content-border">
						See something wrong? Missing data or a bug? Please let me know! <a href="mailto: morrowind@tes3db.com">morrowind@tes3db.com</a>
					</div>
				</div>
			</div>
		<!-- END CONTENT ROW -->
		<!-- BEGIN FOOTER ROW -->
			<div class="row gx-1 my-1 text-center">
				<div class="col">				
					<div class="content-border content">
						
						<p>This data has been queried <span style="font-weight: bold;">
						<?php 
							try{
								require_once __DIR__ . '/config.php';
								$sql = "SELECT table_rows FROM information_schema.tables WHERE table_schema = 'morrowind' AND table_name = 'logs'";
								//$sql = "SELECT COUNT(1) FROM logs";
								$result = $con->query($sql);
								$row   = mysqli_fetch_row($result);
								echo $row[0];
							}catch(Exception $e){
								echo $e->getMessage();
							}
						?>
						</span> times.
						</p>
					</div>
				</div>
			</div>
		<!-- END FOOTER ROW -->	
		</div>
	</body>
</html>