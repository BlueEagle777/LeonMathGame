<?php
require_once('db_connection.php');

// Retrieve car selection, player ID, body, colour, and character from the Unity client
$playerId = $_POST['playerId'];
$carSelection = $_POST['body']; // Updated field name to match Unity coroutine
$colour = $_POST['colour']; // New field
$wheel = $_POST['wheel']; // New field
$character = $_POST['character']; // New field

// Update the 'body', 'colour', 'wheel' and 'character' columns in the 'car' table for the specified player ID
$updateSql = "UPDATE car SET body = $carSelection, colour = $colour, wheel = $wheel, character_type = $character WHERE player_ID = $playerId";

if ($conn->query($updateSql) === TRUE) {
    echo "Car selection updated successfully.";
} else {
    echo "Error updating car selection: " . $conn->error;
}

$conn->close();
?>