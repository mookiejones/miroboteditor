<?php
require_once "Bike.class.php";
/**
 * This file contains Trip class
 * @author Polad Mirzayev <polad.mirzayev@gmail.com>
 * @copyright Copyright (c) 2007, Polad Mirzayev
 * @version 1.0
 * @package cyclestats
 */

/**
 * Class that represents a single Trip record
 */
class Trip
{
	/** @var Integer Contains trip date in Unix timestamp format */
	private $tripDate;
	/** @var Integer Contains trip duration in seconds */
	private $duration = 0;
	/** @var Float Contains trip distance */
	private $distance = 0;
	/** @var Float Contains trip average speed */
	private $avgSpeed = 0;
	/** @var Float Contains trip maximum speed */
	private $maxSpeed = 0;
	/** @var Float Contains odometer reading displaying total distance of all trips travelled so far */
	private $odometer = 0;
	/** @var Integer Contains number of calories burnt */
	private $calories = 0;
	/** @var String contains value showing measurement system being used for the trip data */
	private $unit = "m";
	/** @var Bike Contains an object describing which bike was used during the trip */
	private $bike;
	
	/**
	 * Default constructor for trip object
	 */
	public function Trip()
	{
		
	}
	
	/**
	 * Public method for setting trip date
	 * @param Integer $value Date of trip in Unix timestamp format
	 */
	public function setTripDate($value)
	{
		$this->tripDate = $value;
	}
	
	/**
	 * Public method for getting trip date in Unix timestamp format
	 * @return Integer
	 */
	public function getTripDate()
	{
		return $this->tripDate;
	}
	
	/**
	 * Public method for setting trip duration
	 * @param Integer $value Duration of trip in seconds
	 */
	public function setDuration($value)
	{
		$this->duration = $value;
	}
	
	/**
	 * Public method for getting trip duration in seconds
	 * @return Integer
	 */
	public function getDuration()
	{
		return $this->duration;
	}
	
	/**
	 * Public method for setting trip distance
	 * @param Float $value Total distance of trip
	 */
	public function setDistance($value)
	{
		$this->distance = $value;
	}
	
	/**
	 * Public method for getting trip distance
	 * @return Float
	 */
	public function getDistance()
	{
		return $this->distance;
	}
	
	/**
	 * Public method for setting the average speed of the trip
	 * @param Float $value Average speed
	 */
	public function setAvgSpeed($value)
	{
		$this->avgSpeed = $value;
	}
	
	/**
	 * Public method for getting the average speed of the trip
	 * @return Float
	 */
	public function getAvgSpeed()
	{
		return $this->avgSpeed;
	}
	
	/**
	 * Public method for setting the maximum speed of the trip
	 * @param Float $value Maximum speed
	 */
	public function setMaxSpeed($value)
	{
		$this->maxSpeed = $value;
	}
	
	/**
	 * Public method for getting the maximum speed of the trip
	 * @return Float
	 */
	public function getMaxSpeed()
	{
		return $this->maxSpeed;
	}
	
	/**
	 * Public method for setting the number of calories burnt during the trip
	 * @param Integer $value Number of calories
	 */
	public function setCalories($value)
	{
		$this->calories = $value;
	}
	
	/**
	 * Public method for getting the number of calories burnt during the trip
	 * @return Integer
	 */
	public function getCalories()
	{
		return $this->calories;
	}
	
	/**
	 * Public method for setting the odometer reading displaying total distance of all trips travelled so far
	 * @param Float $value Odometer reading after this trip
	 */
	public function setOdometer($value)
	{
		$this->odometer = $value;
	}
	
	/**
	 * Public method for getting the odometer reading displaying total distance of all trips travelled so far
	 * @return Float
	 */
	public function getOdometer()
	{
		return $this->odometer;
	}
	
	/**
	 * Public method for setting an object describing which bike was used during the trip
	 * @param Bike $value Bike object
	 */
	public function setBike($value)
	{
		
	}
	
	/**
	 * Public method for getting an object describing which bike was used during the trip
	 * @return Bike
	 */
	public function getBike()
	{
		return $this->bike;
	}
}

?>