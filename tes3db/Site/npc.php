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
		<script src="npc.js?<?php echo time(); ?>"></script>
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
				padding: 5px;
			}
			.content-border{
				border: solid 2px #6F541D
			}
			body{
				//background-image: url("img/bg.avif");
				font-family: monospace;
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
			.heading-style{
				
			}
		</style>
 	</head>
	<body style="background-color: #2B1A08;">
		<div class="container">
			<div class="row my-1">
				<div class="col">
					<div class="content-border content px-3">
						<h1 id="NameId"></h1>
						Id: <span id="IdId"></span>
					</div>
				</div>
			</div>
			
			<div class="row gx-1">
		<!-- -- -- -- BEGIN LEFT WIDE CONTENT COLUMN -- -- -- -->
				<div class="col-12 col-md-8 col-lg-9">
				<!-- -- BEGIN ROW 1 -- -->
					<div class="row gx-1 my-1">
					
						<div class="col-12 col-md-6 col-lg-4 ">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Info</div></div>
								<table class="w-100">
									<tbody>
									<tr><td>Race:</td><td id="RaceId"></td></tr>
									<tr><td>Class:</td><td id="ClassId"></td></tr>
									<tr><td>Faction:</td><td id="FactionId"></td></tr>
									<tr><td>Gender:</td><td id="GenderId"></td></tr>
									<tr><td class="p-2"> </td><td> </td></tr>
									<tr><td>Essential:</td><td id="IsEssentialId"></td></tr>
									<tr><td>Persistent:</td><td id="IsPersistentId"></td></tr>
								</tbody>
								</table>
							</div>
						</div>

						<div class="col-12 col-md-8 col-lg-5">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Location</div></div>
								<table class="w-100">
								  <tbody>
									<tr><td>Expansion:</td><td id="ExpansionId"></td></tr>
									<tr><td>Location:</td><td id="LocationId"></td></tr>
									<tr><td>SubLocation:</td><td id="SubLocationId"></td></tr>
									<tr><td>Cell:</td><td id="CellNameId"></td></tr>
									<tr><td>Region:</td><td id="RegionId"></td></tr>
									<tr><td>Interior:</td><td id="IsInteriorId"></td></tr>
								  </tbody>
								</table>
							</div>
						</div>					
						
						<div class="col-12 col-md-6 col-lg-3">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Attributes</div></div>
								<table class="w-100">
								  <tbody>
									<tr><td>Strength:</td><td class="text-end" id="StrengthId"></td></tr>
									<tr><td>Intelligence:</td><td class="text-end" id="IntelligenceId"></td></tr>
									<tr><td>Willpower:</td><td class="text-end" id="WillpowerId"></td></tr>
									<tr><td>Agility:</td><td class="text-end" id="AgilityId"></td></tr>
									<tr><td>Speed:</td><td class="text-end" id="SpeedId"></td></tr>
									<tr><td>Endurance:</td><td class="text-end" id="EnduranceId"></td></tr>
									<tr><td>Personality:</td><td class="text-end" id="PersonalityId"></td></tr>
									<tr><td>Luck:</td><td class="text-end" id="LuckId"></td></tr>
								  </tbody>
								</table>
							</div>
						</div>
				
					</div>
				<!-- -- END ROW 1 -- -->
				<!-- -- BEGIN ROW 2 -- -->
					<div class="row gx-1 my-1">
					
						<div class="col-12 col-md-6 col-lg-4">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Spells</div></div>
								<table class="w-100">
								  <tbody id="SpellsTableId">
								  </tbody>
								</table>
							</div>
						</div>
					
						<div class="col-12 col-md-6 col-lg-5">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Inventory</div></div>
								<table class="w-100">
									<thead>
										<tr>
											<th>Item</th>
											<th>Quantity</th>
										</tr>
									</thead>
								  <tbody id="InventoryTableId">
								  </tbody>
								</table>
							</div>
						</div>

						<div class="col-12 col-md-6 col-lg-3">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Stats</div></div>
								<table class="w-100">
								  <tbody>
									<tr><td>Level:</td><td class="text-end" id="LevelId"></td></tr>
									<tr><td>Health:</td><td class="text-end" id="HealthId"></td></tr>
									<tr><td>Magicka:</td><td class="text-end" id="MagickaId"></td></tr>
									<tr><td>Fatigue:</td><td class="text-end" id="FatigueId"></td></tr>
									<tr><td class="p-2"> </td><td> </td></tr>
									<tr><td>Disposition</td><td class="text-end" id="DispositionId"></td></tr>
									<tr><td>Reputation</td><td class="text-end" id="ReputationId"></td></tr>
									<tr><td>Rank</td><td class="text-end" id="RankId"></td></tr>
								  </tbody>
								</table>
							</div>
						</div>

					</div>
				<!-- -- END ROW 2 -- -->
				<!-- -- BEGIN ROW 2 -- -->
					<div class="row gx-1 my-1">
										
						<div class="col-12 col-md-6 col-lg-4">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">Services</div></div>
								<br>
								<table class="w-100">
									<tbody>
										<tr><td>Bartering Gold:</td><td id="GoldId"></td></tr>
									</tbody>
								</table>
								<br>
								<table class="w-100">
									<tbody id="ServicesTableId">
									</tbody>
								</table>
							</div>
						</div>
						
						<div class="col-12 col-md-6 col-lg-5"> </div>
						
						<div class="col-12 col-md-4 col-lg-3">
							<div class="content-border content h-100">
								<div class="row"><div class="col text-center">AI Data</div></div>
								<table class="w-100">
								  <tbody>
									<tr><td>Hello</td><td class="text-end" id="HelloId"></td></tr>
									<tr><td>Fight</td><td class="text-end" id="FightId"></td></tr>
									<tr><td>Flee</td><td class="text-end" id="FleeId"></td></tr>
									<tr><td>Alarm</td><td class="text-end" id="AlarmId"></td></tr>
								  </tbody>
								</table>
							</div>
						</div>

					</div>
				<!-- -- END ROW 3 -- -->
				</div>
		<!-- -- -- -- END LEFT WIDE CONTENT COLUMN -- -- -- -->
		<!-- -- -- -- BEGIN RIGHT THIN CONTENT COLUMN -- -- -- -->
				<div class="col-12 col-md-4 col-lg-3 my-1">
				<div class="content-border content h-100">
						<div class="row"><div class="col text-center">Skills</div></div>
						<table class="w-100">
						  <tbody>
							<tr><td>Acrobatics:</td><td class="text-end" id="AcrobaticsId"></td></tr>
							<tr><td>Alchemy:</td><td class="text-end" id="AlchemyId"></td></tr>
							<tr><td>Alteration:</td><td class="text-end" id="AlterationId"></td></tr>
							<tr><td>Armorer:</td><td class="text-end" id="ArmorerId"></td></tr>
							<tr><td>Athletics:</td><td class="text-end" id="AthleticsId"></td></tr>
							<tr><td>Axe:</td><td class="text-end" id="AxeId"></td></tr>
							<tr><td>Block:</td><td class="text-end" id="BlockId"></td></tr>
							<tr><td>Blunt Weapon:</td><td class="text-end" id="BluntWeaponId"></td></tr>
							<tr><td>Conjuration:</td><td class="text-end" id="ConjurationId"></td></tr>
							<tr><td>Destruction:</td><td class="text-end" id="DestructionId"></td></tr>
							<tr><td>Enchant:</td><td class="text-end" id="EnchantId"></td></tr>
							<tr><td>Hand To Hand:</td><td class="text-end" id="HandToHandId"></td></tr>
							<tr><td>Heavy Armor:</td><td class="text-end" id="HeavyArmorId"></td></tr>
							<tr><td>Illusion:</td><td class="text-end" id="IllusionId"></td></tr>
							<tr><td>Light Armor:</td><td class="text-end" id="LightArmorId"></td></tr>
							<tr><td>Long Blade:</td><td class="text-end" id="LongBladeId"></td></tr>
							<tr><td>Marksman:</td><td class="text-end" id="MarksmanId"></td></tr>
							<tr><td>Medium Armor:</td><td class="text-end" id="MediumArmorId"></td></tr>
							<tr><td>Mercantile:</td><td class="text-end" id="MercantileId"></td></tr>
							<tr><td>Mysticism:</td><td class="text-end" id="MysticismId"></td></tr>
							<tr><td>Restoration:</td><td class="text-end" id="RestorationId"></td></tr>
							<tr><td>Security:</td><td class="text-end" id="SecurityId"></td></tr>
							<tr><td>Short Blade:</td><td class="text-end" id="ShortBladeId"></td></tr>
							<tr><td>Sneak:</td><td class="text-end" id="SneakId"></td></tr>
							<tr><td>Spear:</td><td class="text-end" id="SpearId"></td></tr>
							<tr><td>Speechcraft:</td><td class="text-end" id="SpeechcraftId"></td></tr>
							<tr><td>Unarmored:</td><td class="text-end" id="UnarmoredId"></td></tr>
						  </tbody>
						</table>
					</div>
				</div>
			<!-- -- -- -- END RIGHT THIN CONTENT COLUMN -- -- -- -->
			</div>
			
			
			<div class="row p-1 content content-border">
				<div class="col text-center">
					<span id="NpcNameForCountId"></span> has been viewed <span id="NpcCountId"></span> time(s).
				</div>

			</div>
		</div>
	</body>
</html>