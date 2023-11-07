<?php
require_once('db_connection.php');

// Retrieve form data
$name = $_POST['name'];
$surname = $_POST['surname'];
$password = $_POST['password'];
$grade = $_POST['grade']; // Retrieve the selected grade from the dropdown menu

// Prepare the INSERT statement for the 'players' table
$insertPlayerSql = "INSERT INTO players (name, surname, password, grade, total_stars, accuracy_plus, accuracy_min, accuracy_mul, xp) VALUES (?, ?, ?, ?, 0, 0, 0, 0, 0)";

$stmt = $conn->prepare($insertPlayerSql);
$stmt->bind_param("sssi", $name, $surname, $password, $grade);

if ($stmt->execute()) {
    // Get the player_ID of the newly inserted player
    $playerID = $stmt->insert_id;

    // Initialize values for race_one to race_nine with zeros and race_boss with false
    $initialValues = "0, 0, 0, 0, 0, 0, 0, 0, 0, false";

    // Prepare the INSERT statements for the 'stars', 'stars_min', and 'stars_mul' tables
    $insertStarsSql = "INSERT INTO stars (player_ID, race_one, race_two, race_three, race_four, race_five, race_six, race_seven, race_eight, race_nine, race_boss, boss_played) VALUES (?, $initialValues, 0)";
    $insertStarsMinSql = "INSERT INTO stars_min (player_ID, race_one, race_two, race_three, race_four, race_five, race_six, race_seven, race_eight, race_nine, race_boss) VALUES (?, $initialValues)";
    $insertStarsMulSql = "INSERT INTO stars_mul (player_ID, race_one, race_two, race_three, race_four, race_five, race_six, race_seven, race_eight, race_nine, race_boss) VALUES (?, $initialValues)";

    // Prepare and execute the INSERT statements for the 'stars', 'stars_min', and 'stars_mul' tables
    $stmtStars = $conn->prepare($insertStarsSql);
    $stmtStars->bind_param("i", $playerID);
    $stmtStarsMin = $conn->prepare($insertStarsMinSql);
    $stmtStarsMin->bind_param("i", $playerID);
    $stmtStarsMul = $conn->prepare($insertStarsMulSql);
    $stmtStarsMul->bind_param("i", $playerID);

    if ($stmtStars->execute() && $stmtStarsMin->execute() && $stmtStarsMul->execute()) {
        // Insert data into the 'car' table with default values
        $insertCarSql = "INSERT INTO car (player_ID, body, colour, wheel, character_type) VALUES (?, 0, 0, 0, 0)";
        $stmtCar = $conn->prepare($insertCarSql);
        $stmtCar->bind_param("i", $playerID);

        if ($stmtCar->execute()) {
            // After successful insertion
            echo "success";;
        } else {
            echo "Error inserting into car: " . $stmtCar->error;
        }
    } else {
        echo "Error inserting into stars: " . $stmtStars->error;
        echo "Error inserting into stars_min: " . $stmtStarsMin->error;
        echo "Error inserting into stars_mul: " . $stmtStarsMul->error;
    }
} else {
    echo "Error inserting into players: " . $stmt->error;
}

// Close prepared statements and the database connection
$stmt->close();
$stmtStars->close();
$stmtStarsMin->close();
$stmtStarsMul->close();
$stmtCar->close();
$conn->close();
?>
