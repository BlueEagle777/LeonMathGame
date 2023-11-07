<?php
// Connect to the database
require_once('db_connection.php');

// Get input data
$name = $_POST["name"];
$surname = $_POST["surname"];
$password = $_POST["password"];

// Query to check login and retrieve player_ID and grade
$sql = "SELECT player_ID, grade FROM players WHERE name = '$name' AND surname = '$surname' AND password = '$password'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // Fetch the player_ID and grade from the result
    $row = $result->fetch_assoc();
    $playerID = $row["player_ID"];
    $grade = $row["grade"];

    // Return both "Success" and the player_ID
    echo "Success:" . $playerID . ":" . $grade;
} else {
    echo "Failure";
}

$conn->close();
?>