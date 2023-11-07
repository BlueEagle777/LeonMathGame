<?php
// Database connection details
require_once('db_connection.php');

// Retrieve the playerID and equation data sent from Unity
$playerID = $_POST["playerID"];

// Retrieve arrays of equation data sent from Unity
$firstNumbers = explode(",", $_POST["FirstNumber"]);
$secondNumbers = explode(",", $_POST["SecondNumber"]);
$playerAnswers = explode(",", $_POST["PlayerAnswer"]);
$isCorrectValues = explode(",", $_POST["IsCorrect"]);
$times = explode(",", $_POST["Time"]);

// Initialize a flag to check for errors
$errorFlag = false;

// Process and insert equations into the database
for ($i = 0; $i < count($firstNumbers); $i++) {
    $date = date("Y-m-d");
    $firstNum = intval($firstNumbers[$i]);
    $secondNum = intval($secondNumbers[$i]);
    $playerAns = intval($playerAnswers[$i]);
    $correct = ($isCorrectValues[$i] === 'True') ? 1 : 0; // Convert to integer
    $time = intval($times[$i]);

    $sql = "INSERT INTO min_hist (player_ID, Date, FirstNum, SecondNum, PlayerAns, Correct, Time) VALUES (?, ?, ?, ?, ?, ?, ?)";
    $stmt = $conn->prepare($sql);

    // Bind parameters
    $stmt->bind_param("isiiiii", $playerID, $date, $firstNum, $secondNum, $playerAns, $correct, $time); // Use 'i' for integers
    $stmt->execute();

    if ($stmt->error) {
        $errorFlag = true;
        echo "Error inserting equation: " . $stmt->error; // Log the specific error
        break; // Stop processing on the first error
    }
}

$stmt->close();

// Now, we'll include the update_accuracy.php code here
// Start update_accuracy.php
// Start update_accuracy.php
$parameter = "min"; // Set the parameter to "plus," "min," or "mul" as needed

// Check if the parameter is "min"
if ($parameter === "min") {
    // Query the "min_hist" table to count correct answers
    $sql = "SELECT SUM(Correct) AS correctCount, COUNT(*) AS totalCount FROM min_hist WHERE player_ID = ?";
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

        // Update the accuracy_min value in the players table for the current player
        $updateSql = "UPDATE players SET accuracy_min = ? WHERE player_ID = ?";
        $updateStmt = $conn->prepare($updateSql);
        $updateStmt->bind_param("ii", $accuracy, $playerID); // Use 'i' for integer
        $updateStmt->execute();
        $updateStmt->close();

        /*if ($conn->affected_rows > 0) {
            echo "Accuracy updated successfully. New accuracy: " . $accuracy . "%";
        } else {
            echo "Failed to update accuracy.";
        }*/

        // Query to count the total number of correct equations in all three tables
        // Query to calculate the total correct for plus_hist
        $plusSql = "SELECT SUM(Correct) FROM plus_hist WHERE player_ID = ?";
        $plusStmt = $conn->prepare($plusSql);
        $plusStmt->bind_param("i", $playerID);
        $plusStmt->execute();
        $plusResult = $plusStmt->get_result();

        // Query to calculate the total correct for min_hist
        $minSql = "SELECT SUM(Correct) FROM min_hist WHERE player_ID = ?";
        $minStmt = $conn->prepare($minSql);
        $minStmt->bind_param("i", $playerID);
        $minStmt->execute();
        $minResult = $minStmt->get_result();

        // Query to calculate the total correct for mul_hist
        $mulSql = "SELECT SUM(Correct) FROM mul_hist WHERE player_ID = ?";
        $mulStmt = $conn->prepare($mulSql);
        $mulStmt->bind_param("i", $playerID);
        $mulStmt->execute();
        $mulResult = $mulStmt->get_result();

        // Fetch the results
        $plusRow = $plusResult->fetch_assoc();
        $minRow = $minResult->fetch_assoc();
        $mulRow = $mulResult->fetch_assoc();

        // Calculate the total correct
        $plusCorrect = (int)$plusRow['SUM(Correct)'];
        $minCorrect = (int)$minRow['SUM(Correct)'];
        $mulCorrect = (int)$mulRow['SUM(Correct)'];

        // Calculate the total correct across all three tables
        $totalCorrect = $plusCorrect + $minCorrect + $mulCorrect;

        // Update the xp value in the players table for the current player
        $xpUpdateSql = "UPDATE players SET xp = ? WHERE player_ID = ?";
        $xpUpdateStmt = $conn->prepare($xpUpdateSql);
        $xpUpdateStmt->bind_param("ii", $totalCorrect, $playerID); // Use 'i' for integer
        $xpUpdateStmt->execute();
        $xpUpdateStmt->close();
    } else {
        echo "No player history found for plus equations.";
    }

    $stmt->close();
} else {
    echo "Invalid parameter provided.";
}
// End update_accuracy.php

// Close the database connection
$conn->close();

if (!$errorFlag) {
    echo "success"; // This will indicate success to the C# code
} else {
    // If there's an error
    echo "error";
}
?>