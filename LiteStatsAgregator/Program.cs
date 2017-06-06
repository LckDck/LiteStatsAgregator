using System;

namespace LiteStatsAgregator
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			//Create database from tables
			CreateDataBase ();
			//Get all statistics

			//Output statistics
		}



		static void CreateDataBase ()
		{
			var table = new DataBase ();
			table.InitializeAsync ().Wait ();
		}
	}
}
