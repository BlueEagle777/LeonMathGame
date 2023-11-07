<?php
require_once('db_connection.php');

// Retrieve player ID from the Unity coroutine
$playerId = $_POST['playerID'];

// Query the 'car' table to get car information for the specified player ID
$sql = "SELECT body, colour, wheel, character_type FROM car WHERE player_ID = $playerId";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // Convert the result to a JSON object
    $carInfo = $result->fetch_assoc();
    echo json_encode($carInfo);
} else {
    // Player not found
    echo json_encode(array("error" => "Player not found"));
}

$conn->close();
?>
