<?php
require_once "MDB2.php";

$dsn = "mysqli://tester:password@localhost/cyclestats";
	
$myDB =& MDB2::factory($dsn);

if (PEAR::isError($myDB)) {
    die($myDB->getMessage());
}

$mySQL= "SELECT * FROM trip";

$res = $myDB->query($mySQL);

if (PEAR::isError($res)) {
    die($res->getMessage());
}
?>
<table border="1">
  <tr>
    <td>Distance</td>
    <td>Trip time</td>
    <td>Avg Speed</td>
    <td>Max Speed</td>
    <td>Odometer</td>
    <td>Calories</td>
  </tr>
<?php while($row = $res->fetchRow(MDB2_FETCHMODE_ASSOC)){ ?>
  <tr>
    <td><?php printf($row["trip_distance"]." "); $row["trip_unit"]=="m" ? printf("km") : printf("m"); ?></td>
    <td><?php printf($row["trip_duration"]." sec"); ?></td>
    <td><?php printf($row["trip_avg_speed"]." "); $row["trip_unit"]=="m" ? printf("km/h") : printf("mph"); ?></td>
    <td><?php printf($row["trip_max_speed"]." "); $row["trip_unit"]=="m" ? printf("km/h") : printf("mph"); ?></td>
    <td><?php printf($row["trip_odometer"]." "); $row["trip_unit"]=="m" ? printf("km") : printf("m"); ?></td>
    <td><?php printf($row["trip_calories"]); ?></td>
  </tr>
<?php } ?>
</table>