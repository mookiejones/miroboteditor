<?php
require_once "MDB2.php";

class DBI
{
	private $dbh;
	private $dsn = "mysqli://tester:password@localhost/cyclestats";
	
	public function DBI()
	{
		$dbh =& MDB2::factory($this->dsn);
	}
}

?>