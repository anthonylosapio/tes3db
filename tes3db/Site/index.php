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
		<script src="trie.js"></script>
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
				color: #c7c7c7;
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
			
			.postDateCol{
				color: #6F541D;
				text-align: right;
				font-size: 0.9em;
			}
			.postTitleCol{
				color: #6F541D;
				text-align: left;
				font-size: 1.2em;
				font-weight: bold;
			}
			.search-input{
				background-color: black;
				color: white;
			}
			.search-result-type{
				color: #c7c7c7;
				font-size: .8em;
			}
			.search-result-name{
				font-size: 1.1em;
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
									<li>Select how you want the data to be aggregated:
										<ul>
											<li> AVG - Results will by sorted by the averages value for the group</li>
											<li> SUM - Results sorted by the sum total for the group</li>
											<li> MIN - Results sorted by the smallest/minimum value found in the group</li>
											<li> MAX - Results sorted by the largest/maximum value gound in the group</li>
											<li> COUNT - Results sorted by the total number of NPCs in the group</li>
										</ul>
									</li>
									<li>Choose which data point you want the aggregation applied to (Level, Health, Intelligence, Long Blade, etc)</li>
									<li>Select how you want the NPCs to be grouped (Class, Faction, Race, Gender, Expansion)</li>
									<li>Adjust the minimum number of NPCs you want to include to prevent small groups from skewing the results</li>
									<li>Expand the filters and un-check anything you want to exclude from the results</li>
									<li>Click the Run Query button</li>
									<li>Examples:
										<ul>
											<li>To find the Average level of NPCs in each Expansion choose: Expansion - AVG - Level</li>
											<li>To find out which faction has the most bartering gold choose: Faction - SUM - Gold</li> 
											<li>To find out where the luckiest NPC lives choose (spoiler, there's a tie): Location - MAX - Luck</li>
										</ul>
									</li>
								</ul>
								<li>Find NPCs</li>
								<ul>
									<li>Select How many NPC's you want to include in the results</li>
									<li>Choose which data point you want the NPC's sorted by</li>
									<li>Expand the filters and un-check anything you want to exclude from the results</li>
									<li>Click the Find NPCs button</li>			
								</ul>
							</ul>
						</div>
					</div>
				</div>
			</div>
		<!-- END RESULTS ROW -->
		<!-- BEGIN CONTENT ROW -->
			<div class="row gx-1 my-1">
				<div class="col-12 col-md-4">
					<div class="content content-border h-100 p-2">
						<div class="text-center">
							Search the database:
							<input type="text" id="objectSearchInput" class="search-input w-100" />
							
						</div>
						<hr>
						<div class="row" id="searchResultsDiv">
						
						</div>
					</div>
				</div>
				<div class="col-12 col-md-8">
					<div class="content content-border">
						<div id="postContainer"></div>
						<div class="d-flex justify-content-end">
							<div class="btn filterBtn" id="pastUpdatesButton" data-state="show"> show past updates</div>
						</div>
					</div>
				</div>
			</div>
			<div class="row gx-1 my-1">
				<div class="col">
					<div class="content content-border">
						<div class="text-center">Notes about the data:</div>
						<p>
						<ul>
							<li><b>13,690</b> NPCs Currently Included in the database from the following game files:
								<ul>
									<li><b>Morrowind.esm</b>, <b>Tribunal.esm</b>, and <b>Bloodmoon.esm</b> - <i>GOTY Edition</i></li>
									<li><b>TR_Mainland.esm</b> - 2025-08-12 release <i>Grasping Fortune</i></li>
									<li><b>Cyr_Main.esm</b> - 2025-05-14 release <i>Abecean Shores</i></li>
									<li><b>Sky_Main.esm</b> - 2025-05-05 release <i>Dragonstar</i></li>
								</ul>
							</li>

							<li>Classes are combined with their service offering equivalent by removing the word <i>serivce</i> from the end of class names (eg. <i>Wise Woman Service</i> becomes just Wise Woman, <i>Smith Service</i> -> Smith etc)</li>
							<li>Class, Race, & Faction names from Project Tamriel mods were altered to be more human readable and match the names from the base game (T_Cyr_MagesGuild -> <i>Mages Guild</i>, TR_Fact_SyvvitTong -> <i>Syvvit Tong</i>)</li>
							<li>All Khajiit racial variations were converted to <i>Khajiit</i> for now, at some point I will update the data to bring those back in</li>
							<li>All alterations to the raw data can be found in the SQL file <a href="https://github.com/anthonylosapio/tes3db/blob/main/tes3db/sql/data_cleanup.sql" target="_blank">here</a>.</li>
							<li><a href="https://github.com/Greatness7/tes3conv">tes3conv</a> was used to convert the game files to json so that NPC's could be extracted</li>
						</ul>
						</p>
					</div>
				</div>
			</div>
			
			<div class="row gx-0 my-1 text-center">
				<div class="col">
					<div class="content content-border">
						<p>See something wrong? Missing data or a bug? Please let me know! <a href="mailto: contact@tes3db.com">contact@tes3db.com</a></p>
						<p>Have a popular landmass mod that doesn't conflict with the base game? Let me know and I will include it.</p>
		
					</div>
				</div>
			</div>
		<!-- END CONTENT ROW -->
		<!-- BEGIN FOOTER ROW -->
			<div class="row gx-1 my-1 text-center">
				<div class="col">				
					<div class="content-border content">

					</div>
				</div>
			</div>
		<!-- END FOOTER ROW -->	
		</div>
	</body>
</html>