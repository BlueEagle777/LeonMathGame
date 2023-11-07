<?php
// Database connection details
require_once('db_connection.php');

// Retrieve the playerID and parameter sent from Unity
$playerID = $_POST["playerID"];
$parameter = $_POST["parameter"];

// Check if the parameter is "plus"
if ($parameter === "plus") {
    // Query the "plus_hist" table to count correct answers
    $sql = "SELECT SUM(Correct) AS correctCount, COUNT(*) AS totalCount FROM plus_hist WHERE player_ID = ?";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("i", $playerID);
    $stmt->execute();
    $result = $stmt->get_result();


    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $correctCount = $row["correctCount"];
        $totalCount = $row["totalCount"];

        // Calculate accuracy (rounded to the nearest integer)
        if ($totalCount > 0) {
            $accuracy = round(($correctCount / $totalCount) * 100);
        } else {
            $accuracy = 0;
        }

        // Update the accuracy_plus value in the players table for the current player
        $updateSql = "UPDATE players SET accuracy_plus = ? WHERE player_ID = ?";
        $updateStmt = $conn->prepare($updateSql);
        $updateStmt->bind_param("ii", $accuracy, $playerID); // Use 'i' for integer
        $updateStmt->execute();
        $updateStmt->close();

        if ($conn->affected_rows > 0) {
            echo "success";
        } else {
            echo "Failed to update accuracy.";
        }
    } else {
        echo "No player history found for plus equations.";
    }

    $stmt->close();
} else {
    echo "Invalid parameter provided.";
}

// Close the database connection
$conn->close();
?>
