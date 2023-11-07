<?php
// Database connection details
require_once('db_connection.php');

// Retrieve the playerID sent from Unity
$playerID = $_POST["playerID"];

// Query to get the XP value for the player
$sql = "SELECT xp FROM players WHERE player_ID = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("i", $playerID);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    $xpValue = $row["xp"];

    echo $xpValue; // Send the XP value back to Unity
} else {
    echo "Player not found"; // Or any other appropriate message
}

$stmt->close();
$conn->close();
?>