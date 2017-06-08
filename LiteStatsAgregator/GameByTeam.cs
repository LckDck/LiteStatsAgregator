using System;
using System.Collections.Generic;

namespace LiteStatsAgregator
{
	public class GameByTeam
	{
		public string Id { get; set;}
		public string TeamId { get; set;}
		public List<int> LevelTimes { get; set;}
		public int CommonTime { get; set;}
		public int TimeToPrevious { get; set;}
		public int Place { get; set;}
		public int Season { get; set;}
	}
}
