<?php
require_once('db_connection.php');

if ($_SERVER["REQUEST_METHOD"] === "POST") {
    $player_ID = $_POST["playerID"];
    $times = explode(",", $_POST["Time"]);
    $positions = explode(",", $_POST["Position"]);

    $stmt = $conn->prepare("INSERT INTO plus_performance (player_ID, time, position) VALUES (?, ?, ?)");
    $stmt->bind_param("iii", $playerID, $time, $position);

    for ($i = 0; $i < count($times); $i++) {
        $playerID = $player_ID; // Use the playerID from the form
        $time = $times[$i];
        $position = $positions[$i];

        if ($stmt->execute() === FALSE) {
            echo "Error: " . $stmt->error;
            break;
        }
    }

    $stmt->close();
    $conn->close();

    echo "success"; // Indicates success to the Unity client
}
?>
