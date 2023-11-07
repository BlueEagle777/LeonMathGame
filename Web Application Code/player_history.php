<!DOCTYPE html>
<html>
<head>
    <title>Player History</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            margin: 0;
            padding: 0;
            background-image: url('Background.png');
            background-size: cover;
        }
        .container {
            display: flex;
            flex-direction: column;
            justify-content: top;
            align-items: center;
            height: 100vh;
        }
        .heading {
            font-size: 48px;
            margin-bottom: 20px;
            color: white;
        }
        table {
            border-collapse: collapse;
            width: 80%;
            max-width: 600px;
            margin-top: 20px;
        }
        th, td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }
        th {
            background-color: #f2f2f2;
        }
        td:first-child {
            background-color: #f2f2f2;
        }
        td:nth-child(2) {
            background-color: #f2f2f2;
        }
        .correct {
            background-color: #a1eaa4;
        }
        .incorrect {
            background-color: #ffb2b2;
        }
        .back-button {
            position: absolute;
            bottom: 10px;
            left: 10px;
            font-size: 24px;
            font-weight: bold;
            text-decoration: none;
            color: black;
            padding: 20px 40px;
            border-radius: 10px;
            background-color: rgba(200, 200, 200);
            transition: transform 0.2s ease;
        }
        .back-button:hover {
            transform: scale(1.1);
        }
    </style>
</head>
<body>
    <a class="back-button" href="student_progress.php">Back to Dashboard</a>
    <div class="container">
        <h1 class="heading">Player History</h1>

        <!-- Plus History Table -->
        <table>
            <tr>
                <th>Date</th>
                <th>Equation</th>
                <th>Player answer</th>
                <th>Solving time [s]</th>
            </tr>
            <?php
            require_once('db_connection.php');

            $playerID = isset($_GET['player_ID']) ? $_GET['player_ID'] : null;

            if ($playerID === null) {
                die("Player ID not provided.");
            }

            // Plus History Query
            $sql = "SELECT * FROM plus_hist WHERE player_ID = ? ORDER BY ID DESC LIMIT 100";

            $stmt = $conn->prepare($sql);
            $stmt->bind_param("i", $playerID);
            $stmt->execute();
            $result = $stmt->get_result();

            if ($result->num_rows > 0) {
                while ($row = $result->fetch_assoc()) {
                    $date = $row["Date"];
                    $equation = $row["FirstNum"] . " + " . $row["SecondNum"];
                    $playerAnswer = $row["PlayerAns"];
                    $correct = $row["Correct"];
                    $solvingTime = $row["Time"];
                    // Divide solving time by 1000 to get seconds and round to 2 decimal places
                    $solvingTime = round($solvingTime / 1000, 2);

                    $cellClass = $correct ? "correct" : "incorrect";

                    echo "<tr>
                            <td>$date</td>
                            <td class='$cellClass'>$equation</td>
                            <td class='$cellClass'>$playerAnswer</td>
                            <td class='$cellClass'>$solvingTime</td>
                        </tr>";
                }
            } else {
                echo "<tr><td colspan='3'>No plus history found for this player.</td></tr>";
            }

            $stmt->close();
            ?>
        </table>

        <!-- Minus History Table -->
        <table>
            <tr>
                <th>Date</th>
                <th>Equation</th>
                <th>Player answer</th>
                <th>Solving time [s]</th>
            </tr>
            <?php
            // Minus History Query
            $stmt = $conn->prepare("SELECT * FROM min_hist WHERE player_ID = ? ORDER BY ID DESC LIMIT 100");
            $stmt->bind_param("i", $playerID);
            $stmt->execute();
            $result = $stmt->get_result();

            if ($result->num_rows > 0) {
                while ($row = $result->fetch_assoc()) {
                    $date = $row["Date"];
                    $equation = $row["FirstNum"] . " - " . $row["SecondNum"];
                    $playerAnswer = $row["PlayerAns"];
                    $correct = $row["Correct"];
                    $solvingTime = $row["Time"];
                    // Divide solving time by 1000 to get seconds and round to 2 decimal places
                    $solvingTime = round($solvingTime / 1000, 2);

                    $cellClass = $correct ? "correct" : "incorrect";

                    echo "<tr>
                            <td>$date</td>
                            <td class='$cellClass'>$equation</td>
                            <td class='$cellClass'>$playerAnswer</td>
                            <td class='$cellClass'>$solvingTime</td>
                        </tr>";
                }
            } else {
                echo "<tr><td colspan='3'>No minus history found for this player.</td></tr>";
            }

            $stmt->close();
            ?>
        </table>

        <!-- Multiplication History Table -->
        <table>
            <tr>
                <th>Date</th>
                <th>Equation</th>
                <th>Player answer</th>
                <th>Solving time [s]</th>
            </tr>
            <?php
            // Multiplication History Query
            $stmt = $conn->prepare("SELECT * FROM mul_hist WHERE player_ID = ? ORDER BY ID DESC LIMIT 100");
            $stmt->bind_param("i", $playerID);
            $stmt->execute();
            $result = $stmt->get_result();

            if ($result->num_rows > 0) {
                while ($row = $result->fetch_assoc()) {
                    $date = $row["Date"];
                    $equation = $row["FirstNum"] . " × " . $row["SecondNum"];
                    $playerAnswer = $row["PlayerAns"];
                    $correct = $row["Correct"];
                    $solvingTime = $row["Time"];
                    // Divide solving time by 1000 to get seconds and round to 2 decimal places
                    $solvingTime = round($solvingTime / 1000, 2);

                    $cellClass = $correct ? "correct" : "incorrect";

                    echo "<tr>
                            <td>$date</td>
                            <td class='$cellClass'>$equation</td>
                            <td class='$cellClass'>$playerAnswer</td>
                            <td class='$cellClass'>$solvingTime</td>
                        </tr>";
                }
            } else {
                echo "<tr><td colspan='3'>No multiplication history found for this player.</td></tr>";
            }

            $stmt->close();
            $conn->close();
            ?>
        </table>
    </div>
</body>
</html>