<!DOCTYPE html>
<html>
<head>
    <title>Student Dashboard</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            text-align: center;
            margin: 0;
            padding: 0;
            background-image: url('Background.png');
            background-size: cover; /* This makes the background image cover the entire viewport */
        }
        .container {
            display: flex;
            flex-direction: column;
            justify-content: top;
            align-items: center;
            height: 100vh;
        }
        .heading {
            font-size: 48px;    /* Made the heading bigger */
            margin-bottom: 20px;
            color: white; /* Set text color to white for better readability on the background image */
        }
        .table {
            width: 95%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        .table th, .table td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }
        .table th {
            background-color: #f2f2f2;
            position: relative;
        }
        .table th .sort-buttons {
            display: flex;
            flex-direction: column;
            position: absolute;
            top: 0;
            right: 0;
        }
        .table th .sort-buttons button {
            font-size: 14px;
            margin-top: 4px;
            background: none;
            border: none;
            cursor: pointer;
            position: relative;
            padding-left: 20px;
        }
        .table th .sort-buttons button::before {
            content: "▲";
            position: absolute;
            left: 4px;
            top: 0;
            color: #007bff;
            font-size: 20px;
            transform: scaleY(0.7);
        }
        .table th .sort-buttons button.desc::before {
            content: "▼";
        }
        .table tr:nth-child(even) {
            background-color: rgba(255, 255, 255, 0.8); /* Set an opaque white background color for even rows */
        }
        .table tr:nth-child(odd) {
            background-color: rgba(200, 200, 200, 0.8); /* Set a different opaque color for odd rows */
        }
        /* Style for the back button */
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
            background-color: rgba(200, 200, 200); /* Light background color for the button */
            transition: transform 0.2s ease; /* Add a smooth transform animation */
        }
        .back-button:hover {
            transform: scale(1.1); /* Scale the button slightly on hover */
        }
    </style>
</head>
<body>
    <a class="back-button" href="index.php">Back to Home</a> <!-- Back button link -->
    <div class="container">
        <h1 class="heading">Student Dashboard</h1>
        <table class="table">
            <tr>
                <th>History</th>
                <th>Name</th>
                <th>Surname</th>
                <th>Password</th>
                <th>Grade
                    <div class="sort-buttons">
                        <form action="" method="get">
                            <button type="submit" name="sort" value="grade_asc" class="asc"></button>
                            <button type="submit" name="sort" value="grade_desc" class="desc"></button>
                        </form>
                    </div>
                </th>
                <th>Stars [/81]
                    <div class="sort-buttons">
                        <form action="" method="get">
                            <button type="submit" name="sort" value="stars_asc" class="asc"></button>
                            <button type="submit" name="sort" value="stars_desc" class="desc"></button>
                        </form>
                    </div>
                </th>
                <th>Plus accuracy [%]</th>
                <th>Minus accuracy [%]</th>
                <th>Multiplication accuracy [%]</th>
            </tr>
            <?php
            require_once('db_connection.php');

            $sort = isset($_GET['sort']) ? $_GET['sort'] : 'grade_asc'; // Get the sorting parameter from URL

            if ($sort === 'grade_asc') {
                $sql = "SELECT * FROM players ORDER BY grade ASC"; // Sort by Grade in ascending order
            } elseif ($sort === 'grade_desc') {
                $sql = "SELECT * FROM players ORDER BY grade DESC"; // Sort by Grade in descending order
            } elseif ($sort === 'stars_asc') {
                $sql = "SELECT * FROM players ORDER BY total_stars ASC"; // Sort by Stars in ascending order
            } elseif ($sort === 'stars_desc') {
                $sql = "SELECT * FROM players ORDER BY total_stars DESC"; // Sort by Stars in descending order
            } else {
                $sql = "SELECT * FROM players ORDER BY grade ASC"; // Default sorting by Grade in ascending order
            }

            $result = $conn->query($sql);

            if ($result->num_rows > 0) {
                $row_count = 0;
                while ($row = $result->fetch_assoc()) {
                    $row_count++;
                    $row_color = $row_count % 2 == 0 ? "even" : "odd";
                    echo "<tr class='$row_color'>
                            <td><a href='player_history.php?player_ID=" . $row["player_ID"] . "'>History</a></td> <!-- History button -->
                            <td>" . $row["name"] . "</td>
                            <td>" . $row["surname"] . "</td>
                            <td>" . $row["password"] . "</td>
                            <td>" . $row["grade"] . "</td>
                            <td>" . $row["total_stars"] . "</td>
                            <td>" . $row["accuracy_plus"] . "</td>
                            <td>" . $row["accuracy_min"] . "</td>
                            <td>" . $row["accuracy_mul"] . "</td>
                        </tr>";
                }
            } else {
                echo "<tr><td colspan='9'>No records found</td></tr>";
            }

            $conn->close();
            ?>
        </table>
    </div>
</body>
</html>