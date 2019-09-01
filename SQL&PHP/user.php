<?php
// Include config file
require_once "config.php";
 
// Define variables and initialize with empty values
 
function HandleNewSession() {
    global $mysqli;

    $UserUUID = trim($_POST["userUUID"]);

    $SessionUUID = trim($_POST["sessionUUID"]);
        // Prepare an insert statement
    $sql = "INSERT INTO session (mSessionUUID,users_idunity) VALUES (?, (SELECT idunity FROM users WHERE mUserUUID = ?))";
 
    if($stmt = $mysqli->prepare($sql)){
        // Bind variables to the prepared statement as parameters
        $stmt->bind_param("ss", $SessionUUID,$UserUUID);
        http_response_code(403);//other error   


        // Attempt to execute the prepared statement
        if($stmt->execute()){
            // Records created successfully. Redirect to landing page
            http_response_code(200);
        } else{
            if($mysqli->errno===1062) { 
                http_response_code(409);//Duplicate
            }
        }
         
        // Close statement
        $stmt->close();
    }
    // Close connection
    $mysqli->close();
}

function HandleNewUser() {
    global $mysqli;
    // Validate name
    $UserUUID = trim($_POST["userUUID"]);
        // Prepare an insert statement
    $sql = "INSERT INTO users (mUserUUID) VALUES (?)";
 
    if($stmt = $mysqli->prepare($sql)){
        // Bind variables to the prepared statement as parameters
        $stmt->bind_param("s", $UserUUID);

        http_response_code(403);//other error   

        // Attempt to execute the prepared statement
        if($stmt->execute()){
            // Records created successfully. Redirect to landing page
            http_response_code(200);
        } else{
            if($mysqli->errno===1062) { 
                http_response_code(409);//Duplicate
            }
        }
         
        // Close statement
        $stmt->close();
    }
    // Close connection
    $mysqli->close();
}
function HandleEvent() {
    global $mysqli;
    // Validate name
    $UserUUID = trim($_POST["userUUID"]);
    $SessionUUID = trim($_POST["sessionUUID"]);
    $Event = trim($_POST["event"]);
    http_response_code(403);//other error
        // Prepare an insert statement
    $sql = "INSERT INTO event (mName,session_idsession,session_users_idunity) VALUES (?, (SELECT idsession FROM session WHERE mSessionUUID = ?), (SELECT idunity from users WHERE mUserUUID = ?))";
 
    if($stmt = $mysqli->prepare($sql)){
        // Bind variables to the prepared statement as parameters
        $stmt->bind_param("sss", $Event,$SessionUUID,$UserUUID);

        // Attempt to execute the prepared statement
        if($stmt->execute()){
            http_response_code(200);
        } else{
            if($mysqli->errno===1062) { 
                http_response_code(409);//Duplicate
            }
        }
         
        // Close statement
        $stmt->close();
    }
    // Close connection
    $mysqli->close();
}

function HandlePOST() { 
    $Command = trim($_POST["command"]);
    switch ($Command)
    {
        case "newuser":
            HandleNewUser();
            break;

        case "newsession":
            HandleNewSession();
            break;

        case "newevent":
            HandleEvent();
            break;
        
        default:
            http_response_code(404);//Command not found
            break;
    }
}

// Processing form data when form is submitted
if($_SERVER["REQUEST_METHOD"] == "POST") {
    HandlePOST();
}


?>
