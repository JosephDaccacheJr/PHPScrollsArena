<?php
require 'ConnectionSettings.php';

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
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
      
      echo json_encode($rows);
    
  } else {
      echo "0";
  } 
  

  $conn->close();
  ?> 