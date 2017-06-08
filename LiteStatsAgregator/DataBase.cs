using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiteStatsAgregator
{
	public class DataBase
	{
		int MAX_SEASON_NO = 1000;
		int MIN_YEAR = 2007;
		int MAX_YEAR = DateTime.Now.Year;

		string GetURL (int year, int season)
		{
			return $"http://lite.dzzzr.ru/kaliningrad/?section=rating&year={year}&season={season}&sgms=1";
		}

		List<int> RatingGameIds = new List<int> ();
		List<GameByTeam> RatingGames = new List<GameByTeam> ();
		Dictionary<string, string> Teams = new Dictionary<string, string> ();

		public DataBase ()
		{
			Console.WriteLine ($"Create database:");
		}


		public async Task InitializeAsync ()
		{

			//создать dictionary команд
			Teams = await ReadTeams ();

			//получить id всех рейтинговых игр 

			RatingGameIds = await ReadGameIds ();

			//получить все рейтинговые игры
			RatingGames = await ReadGames ();

		}


		async Task<List<GameByTeam>> ReadGames ()
		{
			var result = new List<GameByTeam> ();
			var count = 1;
			foreach (var id in RatingGameIds) {
				Console.WriteLine ($"Read game {count++}/{RatingGameIds.Count}");
				var url = $"http://lite.dzzzr.ru/kaliningrad/?section=arc&what=stat&gmid={id}";
				var html = await HtmlReader.Instance.ReadHtml (url);

				var teamIds = GetInGameTeams (html);

				if (teamIds.Any ()) {
					Console.WriteLine ($"... New game type id = {id}");

					if (id == 169) {
						Console.WriteLine (1);
					}

					var places = GetPlaces (html);
					var commonTimes = GetCommonTime (html);
					var timeToPrev = GetTimeToPrevious (html);

					var ind = 0;
					foreach (var teamId in teamIds) {
						//не учитываем дисквалифицированных
						if (ind >= places.Count) {
							break;
						}
						var gameInfo = GetGameInfo (html, id.ToString (), teamId);
						gameInfo.TimeToPrevious = timeToPrev [ind];
						gameInfo.CommonTime = commonTimes [ind];
						gameInfo.Place = places [ind];
						Console.WriteLine ($"  time to prev = {timeToPrev [ind]}");
						Console.WriteLine ($"  common time = {commonTimes [ind]}");
						Console.WriteLine ($"  place = {places [ind]}");
						result.Add (gameInfo);

						ind++;
					}

				} else {
					Console.WriteLine ($"... Old game type id = {id}");
				}


			}
			return result;
		}


		GameByTeam GetGameInfo (string html, string id, string teamId)
		{
			var info = new GameByTeam ();
			info.Id = id;
			info.TeamId = teamId;
			return info;
		}


		List<string> GetInGameTeams (string html)
		{
			var startString = "?section=teams&teamID=";
			var endString = " id=tmname>";
			var result = GetMatches (html, startString, endString);
			return result;
		}


		List<int> GetLevelTimes (string html, string teamId)
		{
			throw new NotImplementedException ();
		}

		List<int> GetPlaces (string html)
		{
			var startString = "<td style='text-align:center' id=fulltime>";
			var endString = "</td>";

			var result = GetMatches (html, startString, endString);
			if (result.Find (item => item.Contains("/")) != null) {
				result = result.Select (item => { return item.Substring (0, item.Length / 2 - 2 + 1); }).ToList ();
			}

			result.RemoveAll (item => item.Any (char.IsLetter));
			return result.Select (item => int.Parse (item)).ToList ();
		}

		List<int> GetCommonTime (string html)
		{
			var startString = "<td id=fulltime>";
			var endString = "</td>";
			var strings = GetMatches (html, startString, endString);
			return strings.Select (item => GetTime (item)).ToList ();
		}

		List<int> GetTimeToPrevious (string html)
		{
			var startString = "<!--delta--><td>";
			var endString = "</td><!--deltaend-->";
			return GetMatches (html, startString, endString).Select (item => {
				return item.Contains ("-") ? "00:00:00" : item;
			}).Select (str => str.Substring (str.Length - 8)).Select (item => GetTime (item)).ToList ();
		}


		int GetTime (string threePartTime)
		{
			TimeSpan ts = TimeSpan.Parse (threePartTime);
			return (int)ts.TotalSeconds;
		}


		Dictionary<int, int> SeasonGameIds = new Dictionary<int, int> ();

		async Task<List<int>> ReadGameIds ()
		{
			var result = new List<int> ();
			var lastSeason = 1;
			for (var year = MIN_YEAR; year <= MAX_YEAR; year++) {
				Console.WriteLine ($".... Getting game ids for year {year} ....");
				for (var season = lastSeason; season < MAX_SEASON_NO; season++) {
					//если нет таблички - значит в в этом году сезон другой.
					lastSeason++;
					var url = GetURL (year, season);
					var html = await HtmlReader.Instance.ReadHtml (url);
					if (IsEmptyTable (html)) {
						break;
					}
					var newGameIds = GetGameIdsFrom (html, season);
					result.AddRange (newGameIds);
				}
			}
			result.Sort ();
			return result;
		}

		async Task<Dictionary<string, string>> ReadTeams ()
		{
			var teamsUrl = "http://lite.dzzzr.ru/kaliningrad/?section=teams";
			var html = await HtmlReader.Instance.ReadHtml (teamsUrl);
			var startString = "<a href=?section=teams&teamID=";
			var endString = "</a>";
			var matches = GetMatches (html, startString, endString);
			Console.WriteLine ("\nTEAMS:\n");

			var result = new Dictionary<string, string> ();
			foreach (var str in matches) {
				var id = str.Substring (0, str.IndexOf (">"));
				var name = str.Substring (str.IndexOf (">") + 1).TrimStart ();
				Console.WriteLine ($"Team id = {id}, {name}");
				result.Add (id, name);
			}
			Console.WriteLine (string.Empty);
			return result;
		}

		List<string> GetMatches (string text, string startString, string endString)
		{
			List<string> matched = new List<string> ();
			int indexStart = 0, indexEnd = 0;
			bool exit = false;
			while (!exit) {
				indexStart = text.IndexOf (startString);
				if (indexStart == -1) {
					break;
				}

				indexEnd = text.IndexOf (endString, indexStart);
				if (indexStart != -1 && indexEnd != -1) {

					var substrStartIndex = indexStart + startString.Length;
					var substrLength = indexEnd - indexStart - startString.Length;

					if (substrStartIndex + substrLength > text.Length) {
						exit = true;
					}

					var substring = text.Substring (substrStartIndex, substrLength);
					if (string.IsNullOrEmpty (substring)) {
						break;
					}

					matched.Add (substring);
					var nextInd = indexEnd + endString.Length;
					text = text.Substring (nextInd);

				} else
					break;
			}
			return matched;
		}


		List<int> GetGameIdsFrom (string html, int season)
		{
			var result = new List<int> ();
			var startString = "&gmid=";
			var endString = " style='color:black'>";
			var matches = GetMatches (html, startString, endString);
			foreach (var str in matches) {
				int id = 0;
				int.TryParse (str, out id);
				if (id != 0) {
					SeasonGameIds.Add (id, season);
					result.Add (id);
				}
			}
			return result;
		}

		bool IsEmptyTable (string html)
		{
			return !html.Contains ("Сыграно<br>игр");
		}
	}
}
