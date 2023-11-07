<?php
require_once('db_connection.php');

// Get the playerID from the Unity application
if (isset($_POST["playerID"])) {
    $playerID = $_POST["playerID"];

    // Construct the SQL query to retrieve the race_boss value
    $sql = "SELECT race_boss FROM stars_mul WHERE player_ID = " . $playerID;

    // Execute the query
    $result = $conn->query($sql);

    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $bossValue = $row["race_boss"];
        echo $bossValue;
    } else {
        echo "Player not found";
    }
} else {
    echo "Invalid playerID parameter";
}

// Close the database connection
$conn->close();
?>