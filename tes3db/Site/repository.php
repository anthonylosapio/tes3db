<?php
class Repository {
	
	private \mysqli $db;
	
	public function __construct(\mysqli $db){
		$this->db = $db;
	}

	public function RunSelectQuery($query): array {
		try {
			$rowset = array();

			$result = $this->db->query($query);
            if ( $result instanceof \mysqli_result ) {
                while ($row = $result->fetch_assoc()) {
                    $rowset[] = $row;
                }
            }else{
				$this->LogSQLError($this->db, $query);
			}
			
            return $rowset;
		} catch (Exception $e) {
			return [$e->getMessage()];
        }
	}

	public function RunInsertQuery($query): string {
		try {

			$result = $this->db->query($query);
 
            if ($result) {
                return "Success";
            }else{
				$this->LogSQLError($this->db, $query);
				return "Something went wrong...";
			}
        } catch (Exception $e) {
            return $e->getMessage();
        }
    }
	
	public function GetNpc($npcId): array{
		try {
			$stmt = $this->db->prepare("SELECT * FROM allnpc WHERE Id = ?");
			$stmt->bind_param("s", $npcId); 
			$stmt->execute();
			$result = $stmt->get_result();
			 
			
			if ( $result instanceof \mysqli_result ) {
                while ($row = $result->fetch_assoc()) {
                    $rowset[] = $row;
                }
            }else{
				$this->LogSQLError($this->db, $query);
			}
			
            return $rowset;
			
        } catch (Exception $e) {
            return $e->getMessage();
        }		
	}
	
	public function GetNpcCount($npcId):int{
		try{
			$stmt = $this->db->prepare("SELECT COUNT(1) AS 'Views' FROM npc_log WHERE Id = ?");
			$stmt->bind_param("s", $npcId); 
			$stmt->execute();
			$result = $stmt->get_result();
			$count = 0;
			if ( $result instanceof \mysqli_result ) {
                while ($row = $result->fetch_assoc()) {
                    $count = $row['Views'];
                }
            }else{
				$this->LogSQLError($this->db, $query);
			}

            return $count;
			
		}catch (Exception $e) {
            return $e->getMessage();
        }	
	}
	
	private function LogSQLError(\mysqli $con, string $query){
		error_log("Query failed: " . $con->error);
		error_log("Error number: " . $con->errno);
		
	}
}
?>