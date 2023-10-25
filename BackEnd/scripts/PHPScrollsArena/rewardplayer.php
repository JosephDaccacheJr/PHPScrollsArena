<?php
require 'ConnectionSettings.php';

$playerID = $_POST["playerID"];
$opponentID = $_POST["opponentID"];

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
  }

$sql = "SELECT * FROM `users` WHERE ID = '" . $playerID . "'";  
$statement = $conn->prepare($sql); 
$statement->execute();
$resultPlayer = $statement->get_result();

$sql2 = "SELECT * FROM `opponents` WHERE id = '" . $opponentID . "'";  
$statement2 = $conn->prepare($sql2); 
$statement2->execute();
$resultOpp = $statement2->get_result();


if($resultPlayer->num_rows == 1 && $resultOpp->num_rows == 1)
{
  while($resultOppRow = $resultOpp->fetch_assoc()){
    $rewardGold = $resultOppRow["rewardGold"];
    $rewardEXP = $resultOppRow["rewardEXP"];
  }
  while($resultPlayerRow = $resultPlayer->fetch_assoc()){
    $playerEXP = $resultPlayerRow["EXP"];
    $playerGold = $resultPlayerRow["gold"];
  }

  $sql3 = "UPDATE users SET gold ='" . ($playerGold + $rewardGold) ."', EXP ='" . ($playerEXP + $rewardEXP) . "' WHERE ID ='" . $playerID . "'";
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

  
$conn->close();
?> 