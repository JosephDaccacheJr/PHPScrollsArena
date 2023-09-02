<?php


require 'ConnectionSettings.php';

// Variables submitted by user
$characterName = $_POST["characterName"];

// Create connection
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
  }
  
  $sql = "SELECT name FROM users WHERE name = '" . $characterName . "'"; // Queary uses one =
  $result = $conn->query($sql);
  
  if ($result->num_rows > 0) {
    // Tell user that the name is already taken

    echo "Username is already taken.";

} else {
  // Insert the user and password into the database
  
    $sql = "INSERT INTO users (name, raceID,armorID,weaponID,STR,AGL,END,SPD,LCK,gold,LVL,EXP)
        VALUES ('". $characterName . "', -1,1,1,50,50,50,50,50,100,1,0)";


    $result = $conn->query($sql);

    $sql2 = "SELECT `ID` FROM `users` ORDER BY `ID` DESC";
    $result2 = $conn->query($sql2);

    if ($result2->num_rows > 0) {
      // output data of each row
      while($row = $result2->fetch_assoc()) {
        $rows[] = $row;
      }

      echo json_encode($rows[0]); // Gives us the ID of the registered user
    
  }


}

?>