<?php
require 'ConnectionSettings.php';

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
  }

    // PREPARED STATEMENT OT PREVENT SQL INJECTIONS USING MYSQLi
    $sql = "SELECT * FROM `armors`"; 
    $statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
    $statement->execute();
    $result = $statement->get_result();

    if ($result->num_rows > 0) {
      // output data of each row
      while($armorRow = $result->fetch_assoc()) {
        $armorRows[] = $armorRow;
      }
      
    $sql2 = "SELECT * FROM `weapons`";
    $statement2 = $conn->prepare($sql2);
    $statement2->execute();
    $result2 = $statement2->get_result();

    if($result2->num_rows > 0)
    {
        while($weaponRow = $result2->fetch_assoc()){
            $weaponRows[] = $weaponRow;
        }
    }
    $items = array("ARMORS" => $armorRows, "WEAPONS" => $weaponRows);
      // After the whole array is created
      echo json_encode($items); 
    
  } else {
      echo "0";
  } 

  $conn->close();
  ?> 