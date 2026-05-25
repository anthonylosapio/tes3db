<?php
require_once __DIR__ . '/config.php';
require_once __DIR__ . '/consts.php';
require_once __DIR__ . '/service.php';
require_once __DIR__ . '/repository.php';

class Controller
{
    private $service;

    public function __construct(Service $service)
    {
        $this->service = $service;
    }

    public function handle()
    {
        try {
            $action = '';
            $data = '';

            if ($_SERVER['REQUEST_METHOD'] === 'POST') {
                $action = trim($_POST['action'] ?? '');
                $data = trim($_POST['data'] ?? '');
            }

            switch ($action) {
                case 'Home':
                    $this->getPageData();
                    break;
                case 'Query':
                    $this->runQuery($data, true); //true - is an aggregation query
                    break;
				case 'List':
                    $this->runQuery($data, false); //false - no aggregate functions, just list the data
					break;
				case 'NPC':
					$this->getNPC($data);
					break;
				case 'Posts':
					$this->getPosts();
					break;
				case 'Search':
					$this->GetSearchObjectArray();
					break;
                default:
                    http_response_code(400);
                    echo json_encode(['error' => 'Unknown action']);
                    break;
            }
        } catch (Exception $e) {
			http_response_code(500);
			echo json_encode(['error' => htmlspecialchars($e->getMessage())]);
		}
	}

    private function getPageData()
    {
        $jsonData = $this->service->GetConstants();
        header('Content-Type: application/json');
        echo $jsonData;
    }
	
	
	private function getSearchObjectArray(){
		$jsonData = $this->service->GetSearchObjectArray();
        header('Content-Type: application/json');
        echo $jsonData;
	}

    private function runQuery(string $data, bool $isAgg)
    {
        $result = $this->service->ValidateJSON($data);

        if ($result['valid']) {
            $resultsSet = $this->service->GetData($data, $isAgg);
			header('Content-Type: application/json');
            echo $resultsSet;
        } else {
            http_response_code(400);
            echo json_encode(['error' => 'Invalid JSON', 'details' => $result['errors']]);
        }
    }
	
	private function getNPC(string $data)
	{
		$jsonData = $this->service->GetNpc($data);
        header('Content-Type: application/json');
        echo $jsonData;
	}
	
	private function getPosts(){
		$jsonData = $this->service->GetPosts();
        header('Content-Type: application/json');
        echo $jsonData;
	}
}

// Instantiate and handle the request
$repo = new Repository($con);
$service = new Service($repo);
$controller = new Controller($service);
$controller->handle();

?>