<?php


require 'ConnectionSettings.php';

// Variables submitted by user
$STR = $_POST["STR"];
$AGL = $_POST["AGL"];
$END = $_POST["END"];
$SPD = $_POST["SPD"];
$LCK = $_POST["LCK"];
$ID = $_POST["ID"];
$RACE = $_POST["RACE"];

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
  }

  $sql = "SELECT ID FROM users WHERE ID = '" . $ID . "'";
  $result = $conn->query($sql);

  if($result->num_rows > 0){

    // Store item price

    //Second SQL (Delete item)
    $sql2 = "UPDATE users SET STR ='" . $STR ."', AGL ='" . $AGL . "', END ='" . $END ."', SPD ='" . $SPD . "', LCK ='" . $LCK . "', raceID ='" . $RACE . "
    ' WHERE id = '". $ID . "'";

    $result2 = $conn->query($sql2);
    if($result2) // Query was successful
    {
        echo "0";
    }
    else
    {
        echo "4";
    }
  } else{
    echo "5";

    $conn->close();
  }

?>