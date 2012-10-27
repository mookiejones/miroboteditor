<?php
require_once "lib/Trip.class.php";
require_once "lib/vcomp.datetime.class.php";

$trip_date = new VCOMP_DateTime("trip_date");

$newTrip = new Trip();
$newTrip->setTripDate($trip_date->getTimestamp());
$newTrip->setDuration($_POST["hours"] * 3600 + $_POST["minutes"] * 60 + $_POST["seconds"]);
$newTrip->setDistance($_POST["distance"]);
$newTrip->setAvgSpeed($_POST["avg_speed"]);
$newTrip->setMaxSpeed($_POST["max_speed"]);
$newTrip->setOdometer($_POST["odometer"]);
$newTrip->setCalories($_POST["calories"]);

?>