<?php
require_once('db_connection.php');

// Retrieve player ID and star counts and boss
$totalStars = $_POST['totalStars'];
$playerID = $_POST['playerID'];
$starCounts = $_POST['starCounts'];
$boss = $_POST['Boss'];

// Prepare and execute the SQL UPDATE statement fro the stars table
$sql = "UPDATE stars SET
    race_one = $starCounts[0],
    race_two = $starCounts[2],
    race_three = $starCounts[4],
    race_four = $starCounts[6],
    race_five = $starCounts[8],
    race_six = $starCounts[10],
    race_seven = $starCounts[12],
    race_eight = $starCounts[14],
    race_nine = $starCounts[16],
    race_boss = $boss
WHERE player_ID = $playerID";

if ($conn->query($sql) === TRUE) {
    echo "Success";
} else {
    //echo "Error: " . $sql . "<br>" . $conn->error;
}

// Prepare and update the sql statement for the players table
$sql = "UPDATE players SET total_stars = $totalStars WHERE player_ID = $playerID";

if ($conn->query($sql) === TRUE) {
    echo "Success";
} else {
    //echo "Error: " . $sql . "<br>" . $conn->error;
}

$conn->close();
?>
