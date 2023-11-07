<?php
require_once('db_connection.php');

$sql = "SELECT name, surname FROM players";
$result = $conn->query($sql);

$data = array();
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $data[] = array(
            "name" => $row['name'],
            "surname" => $row['surname']
        );
    }
}

$conn->close();

$response = array("players" => $data); // Wrap the array in an object
header('Content-Type: application/json');
echo json_encode($response);
?>