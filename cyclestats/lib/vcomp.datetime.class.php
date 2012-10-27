<?php

class VCOMP_DateTime
{
	private $_date;
	private $css;
	private $monthname = array(1=>"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
	private $abbreviated = true;
	
	public function VCOMP_DateTime($name=NULL, $given_date=NULL)
	{
		$this->name = $name;
		
		if(!$given_date){
			if(isset($_POST["{$this->name}"])){
				$this->_date = mktime(
					$_POST["{$this->name}"]["hour"],
					$_POST["{$this->name}"]["minute"],0,
					$_POST["{$this->name}"]["month"],
					$_POST["{$this->name}"]["day"],
					$_POST["{$this->name}"]["year"]
				);
			}
			else {
				$this->_date = time();
			}
		}
		else {
			$this->_date = $given_date;
		}
	}
	
	public function setDate($timestamp)
	{
		$this->_date = $timestamp;
	}
	
	public function getDay()
	{
		return date("j", $this->_date);
	}
	
	public function getMonth()
	{
		return date("n", $this->_date);
	}
	
	public function getYear()
	{
		return date("Y", $this->_date);
	}
	
	public function getDate($format)
	{
		return date($format, $this->_date);
	}
	
	public function getTimestamp()
	{
		return $this->_date;
	}
	
	public function useCSS($classname)
	{
		$this->css = $classname;
	}
	
	public function isMonthAbbreviated($value = true)
	{
		$this->abbreviated = $value;
	}
	
	public function draw($day=1, $month=1, $year=1, $hour=0, $minute=0)
	{
		if($this->css)
		{
			$cssname = "class=\"$this->css\"";
		}
		else
		{
			$cssname = "";
		}
		
		if($day)
		{
			$objday = "<select name=\"" . $this->name . "[day]\" size=\"1\" $cssname><option value=\"\"></option>";
			$this_day = date("j", $this->_date);
			for($i=1; $i<=31; $i++)
			{
				$selected = "";
				if($this_day == $i)
				{
					$selected = "selected";
				}
				$objday .= "<option value=\"".sprintf("%02d", $i)."\" $selected>" . sprintf("%02d", $i) . "</option>";
			}
			$objday .= "</select>";
		}
		
		if($month)
		{
			$objmonth = "<select name=\"" . $this->name . "[month]\" size=\"1\" $cssname><option value=\"\"></option>";
			$this_month = date("n", $this->_date);
			for($i=1; $i<=12; $i++)
			{
				$selected = "";
				if($this_month == $i)
				{
					$selected = "selected";
				}
				if(isset($this->abbreviated) && $this->abbreviated == 1)
				{
					$monthname = substr($this->monthname[$i],0,3);
				}
				else
				{
					$monthname = $this->monthname[$i];
				}
				$objmonth .= "<option value=\"".sprintf("%02d", $i)."\" $selected>". $monthname ."</option>";
			}
			$objmonth .= "</select>";
		}
		
		if($year)
		{
			$objyear = "<select name=\"" . $this->name . "[year]\" size=\"1\" $cssname><option value=\"\"></option>";
			$this_year = date("Y", $this->_date);
			for($i = date("Y",time()); $i <= date("Y",time())+2; $i++)
			{
				$selected = "";
				if($this_year == $i)
				{
					$selected = "selected";
				}
				$objyear .= "<option value=\"". $i ."\" $selected>" . $i . "</option>";
			}
			$objyear .= "</select>";
		}
		
		if($hour)
		{
			$objhour = "<select name=\"" . $this->name . "[hour]\" size=\"1\" $cssname>";
			$this_hour = date("G", $this->_date);
			for($i=0; $i<=23; $i++)
			{
				$selected = "";
				if($this_hour == $i)
				{
					$selected = "selected";
				}
				$objhour .= "<option value=\"$i\" $selected>" . sprintf("%02d", $i) . "</option>";
			}
			$objhour .= "</select>";
		}
		
		if($minute)
		{
			$objminute = "<select name=\"" . $this->name . "[minute]\" size=\"1\" $cssname>";
			$this_minute = date("i", $this->_date);
			for($i=0; $i<=59; $i++)
			{
				$selected = "";
				if($this_minute == $i)
				{
					$selected = "selected";
				}
				$objminute .= "<option value=\"$i\" $selected>" . sprintf("%02d", $i) . "</option>";
			}
			$objminute .= "</select>";
		}
		
		$objdate = $objday . $objmonth . $objyear . " " . $objhour . $objminute;
		
		return $objdate;
	}
}

?>