<?php
error_reporting(E_ALL);
//Add database credentials here
$dbuser = '';
$dbpass = '';
$dbname = '';
$dbhost = 'localhost';

$con=mysqli_connect($dbhost, $dbuser, $dbpass, $dbname);

// Check connection

if (mysqli_connect_errno($con)){

	echo "Failed to connect to MySQL: " . mysqli_connect_error();

}
?>