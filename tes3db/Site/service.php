<?php
Class Service {
	
	private Repository $repo;
	
	public function __construct(Repository $repo){
		$this->repo = $repo;
	}


	public static function GetConstants():string{
		
		$locations = [
			"Morrowind" => BASE_LOCATIONS,
			"Tribunal" => TRIBUNAL_LOCATIONS,
			"Bloodmoon" => BLOODMOON_LOCATIONS,
			"Tamriel Rebuilt" => TAMRIEL_LOCATIONS
		];
		
		$filters = [
			"Race" => RACES,
			"Class" => CLASSES,
			"Faction" => FACTIONS,
			"Gender" => GENDERS,
			"Expansion" => EXPANSIONS,
			"Location" => $locations,
			"Services" => SERVICES
		];
		
		$data = [
			"Filters" => $filters,
			"Group By" => GROUPBY,
			"Agg" => AGG_FUNCTION,
			"Sort By" => SORTBY
		];

		$jsonData = json_encode($data, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES);

		if (json_last_error() !== JSON_ERROR_NONE) {
			die("JSON encoding error: " . json_last_error_msg());
		}
		
		return $jsonData;
	}
	
	public function GetNpc(string $data):string{
		
		try{
			$result = $this->repo->GetNpc($data);
			
			$this->InsertNpcLog($result[0]['Id']);
			
			$count = $this->repo->GetNpcCount($result[0]['Id']);
			
			$object = [
				"data" => $result,
				"count" => $count
			];
			
			return json_encode($object);
		}
		catch(\Exception $e){
			$this->LogException($e);
			return "Something went wrong...";		
		}

	}	
	
	
	public static function IsList(array $arr): bool {
		if (empty($arr)) return true;
		return array_keys($arr) === range(0, count($arr) - 1);
	}

	public static function ValidateJSON(string $jsonString): array {
		// --- 1. Parse ---
		$data = json_decode($jsonString, true);
		if (json_last_error() !== JSON_ERROR_NONE) {
			return ['valid' => false, 'errors' => ['Invalid JSON: ' . json_last_error_msg()]];
		}

		$errors = [];

		// --- 2. Top-level keys ---
		$topRequired = ['selections', 'filters'];
		foreach ($topRequired as $key) {
			if (!array_key_exists($key, $data)) {
				$errors[] = "Missing required top-level key: '$key'";
			}
		}
		$extraTop = array_diff(array_keys($data), $topRequired);
		foreach ($extraTop as $k) {
			$errors[] = "Unexpected top-level key: '$k'";
		}

		// --- 3. selections ---
		if (isset($data['selections'])) {
			$sel = $data['selections'];
			if (!is_array($sel)) {
				$errors[] = "'selections' must be an object";
			} else {
				$selRequired = ['agg', 'sort', 'group', 'limit', 'min'];
				foreach ($selRequired as $key) {
					if (!array_key_exists($key, $sel)) {
						$errors[] = "Missing required key in 'selections': '$key'";
					} elseif (!is_string($sel[$key])) {
						$errors[] = "'selections.$key' must be a string";
					}
				}
				$extraSel = array_diff(array_keys($sel), $selRequired);
				foreach ($extraSel as $k) {
					$errors[] = "Unexpected key in 'selections': '$k'";
				}
			}
		}

		// --- 4. filters ---
		if (isset($data['filters'])) {
			$fil = $data['filters'];
			if (!is_array($fil)) {
				$errors[] = "'filters' must be an object";
			} else {
				// Simple string-array filter keys
				$simpleArrayKeys = ['RACES', 'CLASSES', 'FACTIONS', 'GENDERS', 'EXPANSIONS', 'SERVICES'];
				foreach ($simpleArrayKeys as $key) {
					if (!array_key_exists($key, $fil)) {
						$errors[] = "Missing required key in 'filters': '$key'";
					} elseif (!is_array($fil[$key]) || !self::IsList($fil[$key])) {
						$errors[] = "'filters.$key' must be an array";
					} else {
						foreach ($fil[$key] as $i => $val) {
							if (!is_string($val)) {
								$errors[] = "'filters.$key[$i]' must be a string";
							}
						}
					}
				}

				// LOCATIONS
				if (!array_key_exists('LOCATIONS', $fil)) {
					$errors[] = "Missing required key in 'filters': 'LOCATIONS'";
				} elseif (!is_array($fil['LOCATIONS'])) {
					$errors[] = "'filters.LOCATIONS' must be an object";
				} else {
					$loc = $fil['LOCATIONS'];
					$locKeys = ['BASE_LOCATIONS', 'BLOODMOON_LOCATIONS', 'TRIBUNAL_LOCATIONS', 'TAMRIEL_LOCATIONS'];
					foreach ($locKeys as $key) {
						if (!array_key_exists($key, $loc)) {
							$errors[] = "Missing required key in 'filters.LOCATIONS': '$key'";
						} elseif (!is_array($loc[$key]) || !self::IsList($loc[$key])) {
							$errors[] = "'filters.LOCATIONS.$key' must be an array";
						} else {
							foreach ($loc[$key] as $i => $val) {
								if (!is_string($val)) {
									$errors[] = "'filters.LOCATIONS.$key[$i]' must be a string";
								}
							}
						}
					}
					$extraLoc = array_diff(array_keys($loc), $locKeys);
					foreach ($extraLoc as $k) {
						$errors[] = "Unexpected key in 'filters.LOCATIONS': '$k'";
					}
				}

				$filAllowed = array_merge($simpleArrayKeys, ['LOCATIONS']);
				$extraFil = array_diff(array_keys($fil), $filAllowed);
				foreach ($extraFil as $k) {
					$errors[] = "Unexpected key in 'filters': '$k'";
				}
			}
		}

		return [
			'valid'  => empty($errors),
			'errors' => $errors,
		];
	}

	
	public function GetData(string $json, bool $isAgg): string{
		
		try{
			
			$this->InsertLog();
			
			$query = $this->BuildQuery($json, $isAgg);
			$result = $this->repo->runSelectQuery($query);
			
			$object = [
				"data" => $result
			];
			
			return json_encode($object);
			
		}catch(\Exception $e){
			$this->LogException($e);
			return "Something went wrong...";		
		}
	}
	
	public function InsertLog(){
		
		$ip = $_SERVER['REMOTE_ADDR'];
		$user_agent = $_SERVER['HTTP_USER_AGENT'];
		$query = "INSERT INTO logs (RemoteIP, UserAgent) VALUES ('$ip','$user_agent')";
		$result = $this->repo->RunInsertQuery($query);		
	}
	
	public function InsertNpcLog(string $id){
		
		$ip = $_SERVER['REMOTE_ADDR'];
		$user_agent = $_SERVER['HTTP_USER_AGENT'];
		$query = "INSERT INTO npc_log (RemoteIP, UserAgent, Id) VALUES ('$ip','$user_agent','$id')";
		$result = $this->repo->RunInsertQuery($query);		
	}
	
	private function BuildQuery(string $json, bool $isAgg): string{
		/* There are 2 types of queries, the type of query returned depends on the value of the isAgg bool:
		true : Returns a query using the supplied aggreagate function
		false : Returns a simple SELECT query that lists NPCs.
		*/
		$data = json_decode($json, true);
		
		$TABLE = "allnpc";
		$SELECTS = "";
		$WHERE = "";
		$GROUPBY = "";
		$ORDERBY = "";
		$HAVING = "";
		$LIMIT = "";
		
		$agg_function = $this->ValidateInputs( [$data['selections']['agg']], AGG_FUNCTION, false );
		$agg_value = $this->ValidateInputs( [$data['selections']['sort']], SORTBY, false );
		$group_value = $this->ValidateInputs( [$data['selections']['group']], GROUPBY, false );
		$limit_value = $this->ValidateInputs( [$data['selections']['limit']], LIMITS, false );
		$min_value = (ctype_digit((string) $data['selections']['min'])) ? $data['selections']['min'] : "1" ;

		//merge location filters into single array
		$locations = $data['filters']['LOCATIONS'];
		$locations_merged = [];
		foreach ($locations as $locKey => $locArray) {
			$locations_merged = array_merge($locations_merged, $locArray);
		}
		$const_locations = array_merge(BASE_LOCATIONS, BLOODMOON_LOCATIONS, TRIBUNAL_LOCATIONS, TAMRIEL_LOCATIONS);
		
		if($isAgg){
			$SELECTS = " {$group_value}, COUNT(1) AS `Total NPCs`, MIN({$agg_value}) AS `Min {$agg_value}`, MAX({$agg_value}) AS `Max {$agg_value}`,  AVG({$agg_value}) AS `Avg {$agg_value}`, SUM({$agg_value}) AS `Sum {$agg_value}`";			
		
			$GROUPBY = " GROUP BY {$group_value}";

			$ORDERBY = " ORDER BY {$agg_function}({$agg_value}) DESC ";
			
			$HAVING = " HAVING COUNT(1) >= {$min_value} ";
		}else{
			$SELECTS = " ROW_NUMBER() OVER (ORDER BY {$agg_value} DESC) AS '#', `Name`, `Id`, {$agg_value}, `Class`, `Faction`, `Race`, `Gender`, `Location`, `Expansion` ";
			
			$GROUPBY = "";
		
			$ORDERBY = " ORDER BY {$agg_value} DESC ";
			
			$LIMIT = " LIMIT {$limit_value}";
		}

		$filter_array = [
			"Class" => $this->ValidateInputs($data['filters']['CLASSES'], CLASSES),
			"Faction" => $this->ValidateInputs($data['filters']['FACTIONS'], FACTIONS),
			"Race" => $this->ValidateInputs($data['filters']['RACES'], RACES),
			"Gender" => $this->ValidateInputs($data['filters']['GENDERS'], GENDERS),
			"Expansion" => $this->ValidateInputs($data['filters']['EXPANSIONS'], EXPANSIONS),
			"Location" => $this->ValidateInputs($locations_merged, $const_locations),
		];
		
		$services_array = $data['filters']['SERVICES'];
		
		$WHERE = $this->BuildWhereClause($filter_array, $services_array);

		error_log("| SELECT {$SELECTS} FROM {$TABLE} {$WHERE} {$GROUPBY} {$HAVING} {$ORDERBY} {$LIMIT} |");

		return "SELECT {$SELECTS} FROM {$TABLE} {$WHERE} {$GROUPBY} {$HAVING} {$ORDERBY}  {$LIMIT}";
	}
	
	private static function BuildWhereClause(array $filters, array $services):string{
		
		$WHERE = "WHERE 1=1 ";
		//non-nested filters
		foreach ($filters as $key => $value) {
			if( strlen($value) > 0){
				$WHERE .= " AND {$key} NOT IN ({$value})";
			}
		}
		
		$WHERE .= " AND ( 0=1";
		foreach (SERVICES as $s){
			if( !in_array($s, $services, true) ){
				$WHERE .= " OR {$s}='True' ";
			}
			
		}
		$WHERE .=") ";
		
		return $WHERE;
	}
	
	private static function ValidateInputs(array $inputs, array $possibleValues, bool $quotes=true):string{
		$returnString = "";
		$i = 0;
		$q = "";
		if($quotes) $q = "'";
		foreach ($inputs as $input) {
			if( in_array($input, $possibleValues, true) ){
				if($i > 0) {$returnString .= ", ";}
				$escapedInput = addslashes($input);
				$returnString .= "{$q}{$escapedInput}{$q}";
				$i++;
			}
		}		
		return $returnString;
	}
	
	private static function LogException(\Exception $e){
		error_log("Exception: " . $e->getMessage());
		error_log("File: " . $e->getFile());
		error_log("Line: " . $e->getLine());
		error_log("Trace: " . $e->getTraceAsString());
	}
}

?>