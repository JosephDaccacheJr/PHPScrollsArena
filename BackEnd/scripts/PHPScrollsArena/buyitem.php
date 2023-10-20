<?php
require 'ConnectionSettings.php';

$itemID = $_POST["itemID"];
$itemType = $_POST["itemType"];
$playerID = $_POST["playerID"];

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
  }

  $sql = "SELECT * FROM `" . $itemType . "` WHERE ID = '" . $itemID . "'"; 
  $statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
  $statement->execute();
  $result = $statement->get_result();

  $itemCost;

  if($result->num_rows > 0)
  {
    while($itemRow = $result->fetch_assoc()){
            $itemCost = $itemRow["cost"];
        }
  }
  else
  {
    echo "1";
    $conn->close();
    return;
  }

//  $sql = "SELECT name FROM users WHERE name = '" . $characterName . "'"; // Queary uses one =
  $sql2 = "SELECT * FROM `users` WHERE ID = '" . $playerID . "'";  
  $statement2 = $conn->prepare($sql2); 
  $statement2->execute();
  $result2 = $statement2->get_result();

  $playerGold;
  if($result2->num_rows > 0)
  {
      while($playerRow = $result2->fetch_assoc()){
        $playerGold = $playerRow["gold"];
      }
  }
  if($playerGold >= $itemCost)
  {
    if($itemType == "WEAPONS")
    {
        $type = "weaponID";
    }
    else
    {
        $type = "armorID";
    }
    $sql3 = "UPDATE users SET gold ='" . ($playerGold - $itemCost) ."', " . $type . " ='" . $itemID . "' WHERE ID ='" . $playerID . "'";
    $statement3 = $conn->prepare($sql3); 
    if($statement3->execute())
    {
      echo "0";
    }
    else
    {
      echo "1";
    }
    
  }
  else
  {
    echo "-1";
  }

  
$conn->close();
?> 