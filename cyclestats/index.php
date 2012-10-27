<?php
require_once "lib/vcomp.datetime.class.php";

$trip_date = new VCOMP_DateTime("trip_date");
$trip_date->isMonthAbbreviated(true);
?>
<html>
<head>
<title>Add new Trip</title>
</head>
<body>

<h1>Add New Trip</h1>
<br>
<form name="frmNewTrip" action="addTrip.php" method="POST">
<table>
  <tr>
    <td>unit</td>
    <td>
		<select name="unit">
		  <option value="m">metric</option>
		  <option value="i">imperial</option>
		</select>
	</td>
  </tr>
  <tr>
  	<td>Trip date/time</td>
  	<td>
  	  <?php echo $trip_date->draw(1,1,1,1,1); ?>
  	</td>
  </tr>
  <tr>
  	<td>Trip distance</td>
  	<td>
  	  <input type="text" name="distance" size="20">
  	</td>
  </tr>
  <tr>
  	<td>Trip time</td>
  	<td>
  	  <select name="hours">
  	  	<?php for($i = 0; $i < 24; $i++){ ?>
  	    <option value="<?php echo $i; ?>"><?php printf("%02d",$i); ?></option>
  	    <?php } ?>
  	  </select>
  	  <select name="minutes">
  	  	<?php for($i = 0; $i < 60; $i++){ ?>
  	    <option value="<?php echo $i; ?>"><?php printf("%02d",$i); ?></option>
  	    <?php } ?>
  	  </select>
  	  <select name="seconds">
  	  	<?php for($i = 0; $i < 60; $i++){ ?>
  	    <option value="<?php echo $i; ?>"><?php printf("%02d",$i); ?></option>
  	    <?php } ?>
  	  </select>
  	</td>
  </tr>
  <tr>
  	<td>Average speed</td>
  	<td>
  	  <input type="text" name="avg_speed">
  	</td>
  </tr>
  <tr>
  	<td>Maximum speed</td>
  	<td>
  	  <input type="text" name="max_speed">
  	</td>
  </tr>
  <tr>
  	<td>Calories</td>
  	<td>
  	  <input type="text" name="calories">
  	</td>
  </tr>
  <tr>
  	<td>Odometer</td>
  	<td>
  	  <input type="text" name="odometer">
  	</td>
  </tr>
  <tr>
  	<td></td>
  	<td>
  	  <input type="submit" name="btn_submit" value="Add">
  	  <input type="reset" value="Reset">
  	</td>
  </tr>
</table>
</form>

</body>
</html>