using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteStatsAgregator
{
	public class FirstGamesHardcode
	{
		static Dictionary<string, string> _firstTeams;
		public static Dictionary<string, string> GetFirstTeams ()
		{
			if (_firstTeams != null) return _firstTeams;
			_firstTeams = new Dictionary<string, string> ();


			_firstTeams.Add ("-1", "аDRеналин");
			_firstTeams.Add ("-2", "Альянс-Dream");
			_firstTeams.Add ("-3", "Идиоты");
			_firstTeams.Add ("-4", "Дети Аналога");
			_firstTeams.Add ("-5", "Волшебные маньяки");
			_firstTeams.Add ("-6", "Команда \"D\"");
			_firstTeams.Add ("-7", "Аналогия");
			_firstTeams.Add ("-8", "23 красное");
			_firstTeams.Add ("-9", "39 взвод");
			_firstTeams.Add ("-10", "Зеро");
			_firstTeams.Add ("-11", "ФиНиШисты");
			_firstTeams.Add ("-12", "Белка39");
			_firstTeams.Add ("-13", "23 синие");
			_firstTeams.Add ("-14", "Дети Чубайса");
			_firstTeams.Add ("-15", "Идиотики");
			_firstTeams.Add ("-16", "liteDRive");
			_firstTeams.Add ("-17", "банDеRLоги");
			_firstTeams.Add ("-18", "Отчаянные домохозяйки");
			_firstTeams.Add ("-19", "");
			_firstTeams.Add ("-20", "");
			_firstTeams.Add ("-21", "");
			return _firstTeams;
		}


		static List<GameByTeam> _firstGames;
		public static List<GameByTeam> GetGames (Dictionary<string, string> existingKeys)
		{
			if (_firstGames != null) return _firstGames;

			_firstGames =

			new List<GameByTeam> {
				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId("Дети Аналога", existingKeys),
					LevelTimes = new List<int> {
						GetTime("0:05:56"),
						GetTime("0:26:55"),
						GetTime("0:14:51"),
						GetTime("0:12:27"),
						GetTime("0:33:01"),
						GetTime("0:12:59"),
						GetTime("0:08:35")
					},
					CommonTime = GetTime("1:54:44"),
					TimeToPrevious = 0
				},


				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("АDRеналин", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:31:26"),
						GetTime ("0:21:35"),
						GetTime ("0:19:12"),
						GetTime ("0:33:19"),
						GetTime ("0:23:17"),
						GetTime ("0:25:06"),
						GetTime ("0:23:33")
					},
					CommonTime = GetTime ("2:57:28"),
					TimeToPrevious = GetTime("2:57:28") - GetTime ("1:54:44")
				},


				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("39-й взвод", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:16:51"),
						GetTime ("0:41:30"),
						GetTime ("0:26:54"),
						GetTime ("0:18:26"),
						GetTime ("0:34:19"),
						GetTime ("0:20:06"),
						GetTime ("0:20:19")
					},
					CommonTime = GetTime ("2:58:25"),
					TimeToPrevious = GetTime ("2:58:25") - GetTime ("2:57:28")
				},



				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Аналогия", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:30:23"),
						GetTime ("0:28:07"),
						GetTime ("0:22:37"),
						GetTime ("0:11:25"),
						GetTime ("0:40:51"),
						GetTime ("0:29:56"),
						GetTime ("0:15:43")
					},
					CommonTime = GetTime ("2:59:02") ,
					TimeToPrevious = GetTime ("2:59:02") -  GetTime ("2:58:25")
				},


				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Волшебные маньяки", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:13:05"),
						GetTime ("0:41:18"),
						GetTime ("0:30:40"),
						GetTime ("0:14:31"),
						GetTime ("0:44:19"),
						GetTime ("0:20:02"),
						GetTime ("0:20:57")
					},
					CommonTime = GetTime ("3:04:52"),
					TimeToPrevious = GetTime ("3:04:52") -  GetTime ("2:59:02")
				},

				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("23 синие", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:15:10"),
						GetTime ("0:39:02"),
						GetTime ("1:13:26"),
						GetTime ("0:13:27"),
						GetTime ("1:16:11"),
						GetTime ("0:33:13"),
						GetTime ("0:10:48")
					},
					CommonTime = GetTime ("4:21:17"),
					TimeToPrevious = GetTime ("4:21:17") - GetTime ("3:04:52")
				},

				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Дети Чубайса", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:59:53"),
						GetTime ("0:42:17"),
						GetTime ("1:09:39"),
						GetTime ("0:10:19"),
						GetTime ("0:42:30"),
						GetTime ("0:24:15"),
						GetTime ("1:30:00")
					},
					CommonTime = GetTime ("5:38:53"),
					TimeToPrevious = GetTime ("5:38:53") -  GetTime ("4:21:17")
				},



				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Идиотики", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("1:01:19"),
						GetTime ("0:46:56"),
						GetTime ("1:09:11"),
						GetTime ("0:23:51"),
						GetTime ("0:23:10"),
						GetTime ("0:24:55"),
						GetTime ("1:30:00")
					},
					CommonTime = GetTime ("5:39:22"),
					TimeToPrevious = GetTime ("5:39:22") - GetTime ("5:38:53")
				},

				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("банDеRLоги", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:35:09"),
						GetTime ("1:03:19"),
						GetTime ("0:14:41"),
						GetTime ("0:45:46"),
						GetTime ("0:52:28"),
						GetTime ("0:41:17"),
						GetTime ("1:30:00")
					},
					CommonTime = GetTime ("5:42:40"),
					TimeToPrevious = GetTime ("5:42:40") - GetTime ("5:39:22")
				},


				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("liteDRive", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:17:27"),
						GetTime ("1:29:05"),
						GetTime ("0:45:47"),
						GetTime ("0:21:11"),
						GetTime ("1:21:32"),
						GetTime ("0:22:49"),
						GetTime ("1:30:0")
					},
					CommonTime = GetTime ("6:07:51"),
					TimeToPrevious = GetTime ("6:07:51") - GetTime ("5:42:40")
				},


				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Команда D", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("0:35:40"),
						GetTime ("0:46:46"),
						GetTime ("1:10:28"),
						GetTime ("0:36:26"),
						GetTime ("1:19:56"),
						GetTime ("1:30:00"),
						GetTime ("1:30:00")
					},
					CommonTime = GetTime ("7:29:16"),
					TimeToPrevious = GetTime ("7:29:16") -  GetTime ("6:07:51")
				},




				new GameByTeam {
					Id = "2",
					Season = 1,
					TeamId = GetTeamId ("Отчаянные домохозяйки", existingKeys),
					LevelTimes = new List<int> {
						GetTime ("1:30:00"),
						GetTime ("0:58:01"),
						GetTime ("1:06:10"),
						GetTime ("0:38:49"),
						GetTime ("0:43:03"),
						GetTime ("1:30:00"),
						GetTime ("1:30:00")
					},
					CommonTime = GetTime ("7:56:03"),
					TimeToPrevious = 0
				},

				//new GameByTeam {
				//	Id = "2",
				//	Season = 1,
				//	TeamId = GetTeamId ("", existingKeys),
				//	LevelTimes = new List<int> {
				//		GetTime (""),
				//		GetTime (""),
				//		GetTime (""),
				//		GetTime (""),
				//		GetTime (""),
				//		GetTime (""),
				//		GetTime ("")
				//	},
				//	CommonTime = GetTime (""),
				//	TimeToPrevious = 0
				//},
			};
			return _firstGames;
		}

		static string GetTeamId (string name, Dictionary<string, string> existingKeys)
		{
			string defaultId = string.Empty;
			defaultId = GetFirstTeams ().FirstOrDefault (x => x.Value == name).Key;
			if (string.IsNullOrEmpty (defaultId)) {
				return existingKeys.FirstOrDefault (x => x.Value == name).Key;
			}
			return defaultId;
		}





		static int GetTime (string threePartTime)
		{
			TimeSpan ts = TimeSpan.Parse (threePartTime);
			return ts.Seconds;
		}
	}
}
