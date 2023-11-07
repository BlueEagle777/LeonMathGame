<?php
require_once('db_connection.php');

if ($_SERVER["REQUEST_METHOD"] === "POST") {
    $playerID = $_POST["playerID"];

    $sql = "SELECT time, position FROM plus_performance WHERE player_ID = ?";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("i", $playerID);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result->num_rows > 0) {
        echo "Data loaded successfully\n"; // Indicate success and start data transmission
        while ($row = $result->fetch_assoc()) {
            echo $row["time"] . "," . $row["position"] . "\n";
        }
    } else {
        echo "No data found for playerID: " . $playerID;
    }

    $stmt->close();
    $conn->close();
}
?>
