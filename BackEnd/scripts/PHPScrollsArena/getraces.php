<?php

// This will run the connection settings php file so we don't have to use the samme lines every time
require 'ConnectionSettings.php';

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error); 
  }
  
  // PREPARED STATEMENT OT PREVENT SQL INJECTIONS USING MYSQLi
  $sql = "SELECT * FROM `races`"; 
  $statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
  $statement->execute();
  $result = $statement->get_result();

  if ($result->num_rows > 0) {
    // output data of each row
    while($row = $result->fetch_assoc()) {
      $rows[] = $row;
    }
    // After the whole array is created
    echo json_encode($rows); 
  
} else {
    echo "0";
} 


$conn->close();
?> 