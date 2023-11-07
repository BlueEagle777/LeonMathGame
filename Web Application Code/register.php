<!DOCTYPE html>
<html>
<head>
    <title>Register a New Student</title>
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
        .label {
            font-size: 24px;
            margin-top: 10px;
            color: white;
            display: block; /* Make labels block-level elements */
        }
        .input-box {
            padding: 10px;
            font-size: 24px;
            width: 300px;
            margin: 10px;
            display: block; /* Make input boxes block-level elements */
        }
        .dropdown {
            padding: 10px;
            font-size: 24px;
            width: 325px;
            margin: 10px;
            display: block; /* Make dropdowns block-level elements */
        }
        .button {
            padding: 20px 40px;
            font-size: 24px;
            margin: 20px;
            cursor: pointer;
            text-decoration: none;
            color: white;
            background-color: #007bff;
            border: none;
            border-radius: 10px;
            width: 200px;
            font-weight: bold;
            transition: transform 0.2s ease; /* Add a smooth transform animation */
        }
        .button:hover {
            transform: scale(1.1); /* Scale the button slightly on hover */
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
        #registrationMessage {
            font-size: 24px;
            color: white;
        }
    </style>
</head>
<body>
    <a class="back-button" href="index.php">Back to Home</a> <!-- Back button link -->

    <div class="container">
        <h1 class="heading">Register a new student</h1>
        <form action="register_process.php" method="POST" id="registrationForm">
            <label class="label" for="name">Name:</label>
            <input type="text" id="name" name="name" class="input-box" required>

            <label class="label" for="surname">Surname:</label>
            <input type="text" id="surname" name="surname" class="input-box" required>

            <label class="label" for="password">Password:</label>
            <input type="password" id="password" name="password" class="input-box" required>

            <label class="label" for="confirm_password">Confirm password:</label>
            <input type="password" id="confirm_password" name="confirm_password" class="input-box" required>

            <label class="label" for="grade">Grade:</label>
            <select id="grade" name="grade" class="dropdown" required>
                <option value="1">1</option>
                <option value="2">2</option>
                <option value="3">3</option>
                <option value="4">4</option>
                <option value="5">5</option>
            </select>

            <button type="button" class="button" onclick="registerStudent()">Register</button>
            <p id="registrationMessage"></p>
        </form>
    </div>

    <script>
        function registerStudent() {
            // Get form data
            var name = document.getElementById("name").value;
            var surname = document.getElementById("surname").value;
            var password = document.getElementById("password").value;
            var confirmPassword = document.getElementById("confirm_password").value;
            var grade = document.getElementById("grade").value;

            // Simple validation checks
            if (name === '' || surname === '' || password === '' || confirmPassword === '') {
                // Display an error message if any of the fields are empty
                document.getElementById("registrationMessage").innerHTML = "Please fill in all fields.";
            } else if (password !== confirmPassword) {
                // Display an error message if passwords do not match
                document.getElementById("registrationMessage").innerHTML = "Passwords do not match.";
            } else {
                // Check if the player already exists
                var xhttp = new XMLHttpRequest();
                xhttp.onreadystatechange = function() {
                    if (this.readyState === 4) {
                        if (this.status === 200) {
                            var response = JSON.parse(this.responseText);
                            if (response.exists) {
                                // Display an error message if the player already exists
                                document.getElementById("registrationMessage").innerHTML = "Player already exists.";
                            } else {
                                // Send form data to register_process.php for registration
                                var registrationRequest = new XMLHttpRequest();
                                registrationRequest.onreadystatechange = function() {
                                    if (this.readyState === 4) {
                                        if (this.status === 200) {
                                            // Display the registration message if successful
                                            var registrationMessage = document.getElementById("registrationMessage");
                                            if (this.responseText === "success") {
                                                registrationMessage.innerHTML = name + " registered successfully.";
                                            } else {
                                                // Display an error message if registration fails
                                                registrationMessage.innerHTML = "Registration failed. Please try again.";
                                            }
                                        }
                                    }
                                };
                                registrationRequest.open("POST", "register_process.php", true);
                                registrationRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                                registrationRequest.send("name=" + name + "&surname=" + surname + "&password=" + password + "&confirm_password=" + confirmPassword + "&grade=" + grade);
                            }
                        } else {
                            // Display a generic error message if the check fails
                            document.getElementById("registrationMessage").innerHTML = "Player check failed. Please try again.";
                        }
                    }
                };
                xhttp.open("POST", "check_player.php", true);
                xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
                xhttp.send("name=" + name + "&surname=" + surname);
            }
        }
    </script>
</body>
</html>