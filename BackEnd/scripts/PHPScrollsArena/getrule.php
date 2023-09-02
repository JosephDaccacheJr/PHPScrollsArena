<?php

// This will run the connection settings php file so we don't have to use the samme lines every time
require 'ConnectionSettings.php';

// Variables submitted by user
$rulename = $_POST["ruleName"];

// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error); 
}

// PREPARED STATEMENT OT PREVENT SQL INJECTIONS USING MYSQLi
$sql = "SELECT `rule` FROM `ruletable` WHERE `rulename` = ?"; // ? instead of variable
$statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
$statement->bind_param("s",$rulename); // In this we are telling this object alright this sql is gonna replace this questionmark with a parameter of type string.
// i is integer, b is booleon, etc. If yo uhave two vallues it would be like "ss", loginuser, loginpass
$statement->execute();
$result = $statement->get_result();



if ($result->num_rows > 0) {
  // output data of each row
  // This should only happen once
  while($row = $result->fetch_assoc()) {
        echo $row['rule'];
  }
} else {
  // Fail to get rule
  echo "-1";
}

$conn->close();
?> 