<!DOCTYPE html>
<html>
<head>
    <title>Administrator Page</title>
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
        .form-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 20px;
            background-color: rgba(255, 255, 255, 0.2);
            border-radius: 10px;
        }
        .form-label {
            font-size: 24px;
            margin: 10px;
            color: white;
        }
        .form-input {
            padding: 10px;
            font-size: 18px;
            border: none;
            border-radius: 5px;
        }
        .button {
            padding: 20px 40px;
            font-size: 36px;
            margin: 20px;
            cursor: pointer;
            text-decoration: none;
            color: white;
            background-color: #007bff;
            border: none;
            border-radius: 10px;
            width: 300px;
            font-weight: bold;
            transition: transform 0.2s ease;
        }
        .button:hover {
            transform: scale(1.1);
        }
        .back-button {
            padding: 20px 40px;
            font-size: 36px;
            margin: 20px;
            cursor: pointer;
            text-decoration: none;
            color: white;
            background-color: #FF5733;
            border: none;
            border-radius: 10px;
            width: 300px;
            font-weight: bold;
            transition: transform 0.2s ease;
        }
        .back-button:hover {
            transform: scale(1.1);
        }
    </style>
</head>
<body>
    <div class="container">
        <h1 class=" heading">Administrator Page</h1>
        <?php
        if ($_SERVER["REQUEST_METHOD"] === "POST") {
            if (isset($_POST["login"])) {
                $password = $_POST["password"];
                if ($password === "TonyStark") {
                    echo '
                    <form class="form-container" method="post" action="Administrator.php">
                        <label class="form-label">Enter something:</label>
                        <input class="form-input" type="text" name="adminInput" required>
                        <input class="button" type="submit" name="submit" value="Submit">
                    </form>
                    ';
                } else {
                    echo '<p class="form-label">Incorrect password. Try again.</p>';
                }
            } elseif (isset($_POST["submit"])) {
                $adminInput = $_POST["adminInput"];
                
                // Connect to database
                require_once('db_connection.php');

                // Set the $Sql variable equal to the text entered in the form
                $Sql = $_POST["adminInput"];
                
                // Perform the SQL query
                $stmt = $conn->prepare($Sql);

                // Echo back success or error message
                if ($stmt->execute()) {
                    echo '<p class="form-label">Query successful.</p>';
                } else {
                    echo '<p class="form-label">Query error.</p>';
                }
            }
        } else {
            echo '
            <form class="form-container" method="post" action="Administrator.php">
                <label class="form-label">Enter password:</label>
                <input class="form-input" type="password" name="password" required>
                <input class="button" type="submit" name="login" value="Login">
            </form>
            ';
        }
        ?>
        <a href="index.php" class="back-button">Back</a>
    </div>
    <script>
        // JavaScript to hide/show the second form
        const loginButton = document.querySelector('[name="login"]');
        const formContainer = document.querySelector('.form-container');

        loginButton.addEventListener('click', function () {
            const passwordInput = document.querySelector('[name="password"]').value;
            if (passwordInput === 'TonyStark') {
                formContainer.style.display = 'block';
            } else {
                formContainer.style.display = 'none';
            }
        });
    </script>
</body>
</html>
