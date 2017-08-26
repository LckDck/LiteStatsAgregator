using System;
using System.Linq;

namespace LiteStatsAgregator
{
	class MainClass
	{
		public static void Main (string [] args)
		{
			//Console.WriteLine ("Введите название команды:");
			//Team = Console.ReadLine ();
			//Create database from tables
			CreateDataBase ();
			//Get all statistics

			//Output statistics
		}

		static DataBase _table;


		static void CreateDataBase ()
		{
			_table = new DataBase ();
			_table.InitializeAsync ().Wait ();

		}
	}
}
