<?php

// This will run the connection settings php file so we don't have to use the samme lines every time
require 'ConnectionSettings.php';

$command = $_POST["command"];
$opponentID = $_POST["opponentID"];
$playerID = $_POST["playerID"];
$moveType = $_POST["moveType"];
$attackBonus = isset($_POST["attackBonus"]) ? $_POST["attackBonus"] : 0;
$damageBonus = isset($_POST["damageBonus"]) ? $_POST["damageBonus"] : 0;
$defenseBonus = isset($_POST["defenseBonus"]) ? $_POST["defenseBonus"] : 0;

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
  }

  $sql = "SELECT * FROM `opponents` WHERE `ID` = ?"; // ? instead of variable$statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method
  $statement = $conn->prepare($sql); // Statement comes from our connection. Also note -> is like a . in C#, using a property or method    
  $statement->bind_param("i",$opponentID); // In this we are telling this object alright this sql is gonna replace this questionmark with a parameter of type string.
  // i is integer, b is booleon, etc. If yo uhave two vallues it would be like "ss", characterName, loginpass
  $statement->execute();
  $resultOpp = $statement->get_result();

  $sql = "SELECT * FROM `users` WHERE `ID` = ?";
  $statement = $conn->prepare($sql);    
  $statement->bind_param("i",$playerID);
  $statement->execute();
  $resultPlayer = $statement->get_result();
  
  $sql = "SELECT * FROM `weapons`";
  $statement = $conn->prepare($sql);    
  $statement->execute();
  $resultWeapons = $statement->get_result();
  
  $sql = "SELECT * FROM `armors`";
  $statement = $conn->prepare($sql);    
  $statement->execute();
  $resultArmors = $statement->get_result();

  if($resultOpp->num_rows <= 0 || $resultPlayer->num_rows <= 0)
  {
    echo "Database error";
    $conn->close();
  }

  session_start();

  switch($command)
  {
    case "startBattle":
      
      while($row = $resultPlayer->fetch_assoc()) 
      {
        $_SESSION["playerStats{$playerID}"] = setStats($_SESSION["playerStats{$playerID}"],$row,$playerID);
      }

      while($row = $resultOpp->fetch_assoc()) 
      {
        $_SESSION["opponentStats{$opponentID}+{$playerID}"] = setStats($_SESSION["opponentStats{$opponentID}+{$playerID}"],$row,$opponentID);
        $_SESSION["opponentAggression{$opponentID}+{$playerID}"] = $row["aggression"];     
      }

      // GET ARMOR INFO
      while($armorRow = $resultArmors->fetch_assoc())
      {
        $_SESSION["playerStats{$playerID}"] = checkArmor($_SESSION["playerStats{$playerID}"],$armorRow);
        $_SESSION["opponentStats{$opponentID}+{$playerID}"] = checkArmor($_SESSION["opponentStats{$opponentID}+{$playerID}"],$armorRow);
      }

      // GET WEAPON INFO
      while($weaponRow = $resultWeapons->fetch_assoc())
      {
        $_SESSION["playerStats{$playerID}"] = checkWeapon($_SESSION["playerStats{$playerID}"],$weaponRow);
        $_SESSION["opponentStats{$opponentID}+{$playerID}"] = checkWeapon($_SESSION["opponentStats{$opponentID}+{$playerID}"],$weaponRow);                
      }

      // GET AGGRESSION 
            
            echo "HP has been set";
      break;

      case "updateBattle":
        $oppHP =  $_SESSION["opponentStats{$opponentID}+{$playerID}"]["HP"];
        $playerHP = $_SESSION["playerStats{$playerID}"]["HP"];
        $oppName = $_SESSION["opponentStats{$opponentID}+{$playerID}"]["name"];
        $playerName = $_SESSION["playerStats{$playerID}"]["name"];
        $flourish = $_SESSION["playerStats{$playerID}"]["flourish"];
        $hpData[] = array("opponentHP" => $oppHP,"playerHP" => $playerHP,"opponentName" => $oppName,"playerName" => $playerName,"playerFlourish" => $flourish,);
        echo json_encode($hpData);
      break;
      case "playerMove":
        performAttack($moveType,"playerStats{$playerID}", "opponentStats{$opponentID}+{$playerID}");
        break;
      case "opponentMove":
        $aggression = $_SESSION["opponentAggression{$opponentID}+{$playerID}"];
        $curHP = $_SESSION["opponentStats{$opponentID}+{$playerID}"]["HP"];
        $maxHP = $_SESSION["opponentStats{$opponentID}+{$playerID}"]["maxHP"];

        $decisionDice = $aggression + 
        (($curHP / $maxHP) * 50)
        + rollDice(1,50);

        if($decisionDice <= 50 && ($curHP <= ($maxHP * 0.5)))
        {
          defendAndHeal("opponentStats{$opponentID}+{$playerID}");
        }
        else
        {
          $decisionDice = $aggression + rollDice(1,100);
          if($decisionDice <= 60)
          {
            performAttack("quickAttack", "opponentStats{$opponentID}+{$playerID}","playerStats{$playerID}");
          }
          else if($decisionDice >= 140)
          {
            performAttack("powerAttack", "opponentStats{$opponentID}+{$playerID}","playerStats{$playerID}");
          }
          else
          {
            performAttack("regularAttack", "opponentStats{$opponentID}+{$playerID}","playerStats{$playerID}");
          }
        }
        break;

      default:
        "Invalid command";
      break;
  }

$conn->close();


?> 
<?php

function rollDice($count,$sides)
{
  $rollTotal = 0;
  for($i = 0; $i < $count; $i++)
  {
    $rollTotal += rand(1,$sides);
  }
  return $rollTotal;
}

function setStats($character,$row,$playerID)
{
  $hp = (int)((($row["STR"] / 10) + ($row["END"] / 2)) * $row["LVL"] );
  $character = array("ID" => $playerID, "name" => $row["name"], "maxHP" => $hp, "HP" => $hp,
                                        "STR" => $row["STR"], "AGL" => $row["AGL"], "END" => $row["END"],"SPD" => $row["SPD"],"LCK" => $row["LCK"],
                                        "weaponID" => $row["weaponID"], "armorID" => $row["armorID"],
                                        "weaponStat" => 0, "armorStat" => 0,
                                        "dmgNum" => 0, "dmgSides" => 0,
                                        "AC" => 0,
                                        "flourish" => 0, "attackBonus" => 0, "damageBonus" => 0, "defenseBonus" => 0);
  return $character;
}

function checkArmor($character,$armorRow)
{
    if($armorRow["id"] === $character["armorID"])
    {
      if($armorRow["govstat"] === "AGL")
      {
        $character["armorStat"] = $character["AGL"];
      }
      else
      {
        $character["armorStat"] = $character["STR"];
      }
      $character["AC"] = $armorRow["AC"];
    }

    return $character;
}

function checkWeapon($character,$weaponRow)
{
  if($weaponRow["id"] === $character["weaponID"])
  {
    echo "Found Player Weapon\n";
    if($weaponRow["govstat"] === "AGL")
    {
      $character["weaponStat"] = $character["AGL"];
    }
    else
    {
      $character["weaponStat"] = $character["STR"];
    }
    $character["dmgNum"] = $weaponRow["dmgnum"];
    $character["dmgSides"] = $weaponRow["dmgsides"];
  }  
  return $character;
}

function getAttackRoll($attacker)
{
  return (rollDice(1,20)) + ($attacker["weaponStat"] / 10) + ($attacker["LCK"] / 10) + $attacker["attackBonus"];
}

function getAC($defender)
{
  return ($defender["SPD"] / 5) + ($defender["armorStat"] / 10) + ($defender["LCK"] / 10)
  + $defender["AC"] + $defender["defenseBonus"];
}

function getDamage($attacker)
{
  $damage = rollDice($attacker["dmgNum"],$attacker["dmgSides"]) + ($attacker["flourish"] * 5) + $attacker["damageBonus"];
  
  if($damage < 1)
  {
    $damage = 1;
  }
  return (int)$damage;
}

function heal($healer)
{
  return rollDice(1,10) + ($healer["END"] / 20) + ($healer["LCK"] / 25);
}

function performAttack($moveType, $attacker, $defender)
{
  switch($moveType)
  {
    case "regularAttack":
      $_SESSION[$attacker]["attackBonus"] = 0;
      $_SESSION[$attacker]["damageBonus"] = 0;
      $_SESSION[$attacker]["defenseBonus"] = 0;
      break;
    case "quickAttack":
      $_SESSION[$attacker]["attackBonus"] = 10;
      $_SESSION[$attacker]["damageBonus"] = -5;
      $_SESSION[$attacker]["defenseBonus"] = 0;
      break;
    case "powerAttack":
      $_SESSION[$attacker]["attackBonus"] = -5;
      $_SESSION[$attacker]["damageBonus"] = 10;
      $_SESSION[$attacker]["defenseBonus"] = 0;
      break;
    case "defendAndHeal":
      defendAndHeal($attacker);
      return;
    case "buildFlourish":
      buildFlourish($attacker);
      return;
      break;
  }

  $hitRoll = intval(getAttackRoll($_SESSION[$attacker]));
  $enemyAC = intval(getAC($_SESSION[$defender]));
        
  if($hitRoll > $enemyAC)
  {
    $damage = getDamage($_SESSION[$attacker]);

    $_SESSION[$defender]["HP"] -= $damage;
    if($_SESSION[$defender]["HP"] <= 0)
    {
      $attackResult[] = array("RESULT" => "WIN","DAMAGE" => $damage,"ENEMYAC" => $enemyAC,"ATTACKTYPE" => $moveType,"HITROLL" => $hitRoll,);
      unset($_SESSION[$defender]);
      unset($_SESSION[$attacker]);
      echo json_encode($attackResult);
    }
    else
    {
      $attackResult[] = array("RESULT" => "HIT","DAMAGE" => $damage,"ENEMYAC" => $enemyAC,"ATTACKTYPE" => $moveType,"HITROLL" => $hitRoll,);
      echo json_encode($attackResult);
    }
  }
  else
  {
    $attackResult[] = array("RESULT" => "MISS","DAMAGE" => 0,"ENEMYAC" => $enemyAC,"ATTACKTYPE" => $moveType,"HITROLL" => $hitRoll,);
    echo json_encode($attackResult);
  }

  $_SESSION[$attacker]["flourish"] = 0;
  $_SESSION[$attacker]["attackBonus"] = 0;
  $_SESSION[$attacker]["damageBonus"] = 0;
  $_SESSION[$attacker]["defenseBonus"] = 0;
  //echo "{$hitRoll} {$enemyAC}";
  //echo "{$_SESSION["playerStats{$playerID}"]["weaponStat"]} {$_SESSION["playerStats{$playerID}"]["armorStat"]} {$_SESSION["opponentStats{$opponentID}"]["weaponStat"]} {$_SESSION["opponentStats{$opponentID}"]["armorStat"]}";
}

function defendAndHeal($character)
{
  $healAmount = intval(heal($_SESSION[$character]));
  echo "Healed {$healAmount}";
  $_SESSION[$character]["HP"] += $healAmount;
  if($_SESSION[$character]["HP"] > $_SESSION[$character]["maxHP"])
  {
    $_SESSION[$character]["HP"] = intval($_SESSION[$character]["maxHP"]);
  }
  $_SESSION[$character]["defenseBonus"] = 10;

  $healResult[] = array("RESULT" => "HEALED","HEALAMOUNT" => $healAmount,);
  echo json_encode($healResult);
  
}

function buildFlourish($character)
{
  $_SESSION[$character]["flourish"] += 1;
  $_SESSION[$character]["defenseBonus"] = 0;
  $buildResult[] = array("RESULT" => "FLOURISHBUILT", "FLORUISH" => $_SESSION[$character]["flourish"],"CHARACTER" => $character,);
  echo json_encode($buildResult);
}

?>
