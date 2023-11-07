<?php
require_once('db_connection.php');

// Retrieve form data (name and surname)
$name = $_POST['name'];
$surname = $_POST['surname'];

// Prepare and execute a query to check if the player already exists
$stmt = $conn->prepare("SELECT COUNT(*) AS player_count FROM players WHERE name = ? AND surname = ?");
$stmt->bind_param("ss", $name, $surname);
$stmt->execute();
$result = $stmt->get_result();
$row = $result->fetch_assoc();

// Create a response array
$response = array();

if ($row['player_count'] > 0) {
    // Player already exists
    $response['exists'] = true;
} else {
    // Player does not exist
    $response['exists'] = false;
}

// Send JSON response
header('Content-Type: application/json');
echo json_encode($response);

$stmt->close();
$conn->close();
?>