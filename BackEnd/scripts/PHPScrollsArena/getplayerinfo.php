<?php

// This will run the connection settings php file so we don't have to use the samme lines every time
require 'ConnectionSettings.php';

// Variables submitted by user
$playerID = $_POST["playerID"];

// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error); 
}

//  $sql = "SELECT name FROM users WHERE name = '" . $characterName . "'"; // Queary uses one =
$sql = "SELECT * FROM `users` WHERE ID = '" . $playerID . "'";  
$statement = $conn->prepare($sql); 
$statement->execute();
$result = $statement->get_result();

$playerGold;
if($result->num_rows > 0)
{
    while($playerRow = $result->fetch_assoc()){
      $playerInfo = array("NAME" => $playerRow["name"], "GOLD" => $playerRow["gold"], "LVL" => $playerRow["LVL"], "WEAPONID" => $playerRow["weaponID"],
    "ARMORID" => $playerRow["armorID"], "RACEID" => $playerRow["raceID"]);
      echo json_encode($playerInfo);
    }
}

$conn->close();
?> 