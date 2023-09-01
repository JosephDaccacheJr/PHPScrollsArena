<?php
require 'ConnectionSettings.php';

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); //This is the equivilent to return in C#
  }
  
    // PREPARED STATEMENT OT PREVENT SQL INJECTIONS USING MYSQLi
    $sql = "SELECT * FROM `opponents`"; 
    $statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
    $statement->execute();
    $result = $statement->get_result();
  
    if ($result->num_rows > 0) {
      // output data of each row
      while($row = $result->fetch_assoc()) {
        $rows[] = $row;
      }
      // After the whole array is created
      echo json_encode($rows); // This passes an array. Must be an array?
    
  } else {
      echo "0";
  } 
  

  $conn->close();
  ?> 