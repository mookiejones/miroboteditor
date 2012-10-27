<?php
require_once "../lib/Tracker.class.php";
//require_once "PHPUnit/Framework.php";


class testTracker extends PHPUnit_Framework_TestCase
{
	private $myTracker;
	
	public function setUp()
	{
		$this->myTracker = new Tracker();
	}
	
	public function testAddTrip()
	{
		$myTrip = new Trip();
		$this->myTracker->addTrip($myTrip);
		$this->assertEquals(1, $this->myTracker->getNoTrips());
	}
	
	public function testGetTrips()
	{
		$newTrip = new Trip();
		$this->myTracker->addTrip($newTrip);
	}
}

$myTest = new testTracker();
$myTest->setUp();
$myTest->testAddTrip();
$myTest->testGetTrips();

?>