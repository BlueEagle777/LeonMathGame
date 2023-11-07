<?php
// Database connection details
require_once('db_connection.php');

// Retrieve the playerID sent from Unity
$playerID = $_POST["playerID"];

// Query to increment the boss_played column for the player
$updateSql = "UPDATE stars SET boss_played = boss_played + 1 WHERE player_ID = ?";
$stmt = $conn->prepare($updateSql);
$stmt->bind_param("i", $playerID);
$stmt->execute();

if ($conn->affected_rows > 0) {
    echo "success"; // Indicate a successful update to Unity
} else {
    echo "error"; // Indicate an error to Unity
}

$stmt->close();
$conn->close();
?>
