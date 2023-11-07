<!DOCTYPE html>
<html>
<head>
    <title>Welcome Teachers</title>
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
        .button-container {
            display: flex;
            flex-direction: column;
            align-items: center;
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
            transition: transform 0.2s ease; /* Add a smooth transform animation */
        }
        .button:hover {
            transform: scale(1.1); /* Scale the button slightly on hover */
        }
    </style>
</head>
<body>
    <div class="container">
        <h1 class="heading">Welcome teachers!</h1> <!-- Heading at the top -->
        <div class="button-container">
            <a href="register.php" class="button">Register student</a>
            <a href="student_progress.php" class="button">Student progress</a>
            <a href="Administrator.php" class="button">Admin login</a>
        </div>
    </div>
</body>
</html>