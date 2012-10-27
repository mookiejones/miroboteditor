<?php
require_once "DBI.class.php";
require_once "Trip.class.php";

class Tracker
{
	/** Test */
	private $trips;
	private $dbi;
	
	// Adds a new Trip record to the stats
	public static function addTrip(Trip $record)
	{
		$dbi = new DBI();
		
		$this->trips[] = $record;
		$mySQL = "INSERT INTO trip (trip_duration, trip_distance, trip_avg_speed, trip_max_speed, trip_odometer, trip_calories, trip_unit) VALUES (" .
				($_POST["hours"] * 3600 + $_POST["minutes"] * 60 + $_POST["seconds"]) . ", " .
				$_POST["distance"] . ", " .
		 $_POST["avg_speed"] . ", " .
		 $_POST["max_speed"] . ", " .
		 $_POST["odometer"] . ", " .
		 $_POST["calories"] . ", '" .
		 $_POST["unit"] . "')";
*/
	}
	
	// Returns the number of trips
	public function getNoTrips()
	{
		return sizeof($this->trips);
	}
	
	public function getTrips()
	{
		return $this->trips;
	}
	
	public function getTrips($tripId)
	{
		
	}
}

?>