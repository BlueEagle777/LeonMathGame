<?php
require_once('db_connection.php');

$name = $_POST['name'];
$surname = $_POST['surname'];

// Query to retrieve star counts
$sql = "SELECT race_one, race_two, race_three, race_four, race_five, race_six, race_seven, race_eight, race_nine FROM stars_mul WHERE player_ID IN (SELECT player_ID FROM players WHERE name = '$name' AND surname = '$surname')";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
    // Fetch the star counts and send them as a response
    $row = $result->fetch_assoc();
    echo json_encode($row);
} else {
    // Player not found
    echo "Player not found";
}

$conn->close();
?>