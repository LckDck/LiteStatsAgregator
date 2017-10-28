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
        string DeletedTeamName = "*TEAM DELETED*";

        string Lite = "lite";
        string Classic = "classic";

        int StartSeasonNumber = 1;

        string Type {
            get {
                return Lite;
            }
        }

        string GetURL (int year, int season)
        {
            return $"http://{Type}.dzzzr.ru/kaliningrad/?section=rating&year={year}&season={season}&sgms=1";
        }

        string GetURL (int year)
        {
            return $"http://{Type}.dzzzr.ru/kaliningrad/?section=rating&year={year}&sgms=1";
        }

        public List<Game> Games = new List<Game> ();
        public List<GameByTeam> GamesByTeam = new List<GameByTeam> ();
        public Dictionary<string, string> Teams = new Dictionary<string, string> ();

        public DataBase ()
        {
            Console.WriteLine ($"Create database:");
        }


        public async Task InitializeAsync ()
        {

            //создать dictionary команд
            Teams = await ReadTeams ();

            //получить id всех рейтинговых игр 

            Games = await ReadGames ();


            //получить все рейтинговые игры
            GamesByTeam = await ReadTeamGames ();

            ReadBestTimes ();


            CreateStatistics ();
        }

        void CreateStatistics ()
        {

            var teams = new List<string> {
                "Pepperhouse",
				//"Ну хоть какой-нибудь секс..."
				//"Pepperhouse",
				//"Полтора Землекопа",
				//"БЕШЕНЫЙ ХОМЯК",
				//"ЧЗХ?!", "WTF !?",
				//"Я И БАЛ ВЫПУСКНИЦ",
				//"Палата N6",
				//"2goDoom",
				//"Goroshek, luche tikovka",
				//"Йопта",
				//"C-IV",
				//"Пацы из Чичиваро-Данго",
				//"Усы, лапы и хвост",
				//"Синяя яма"
			};

            //var teamids = new List<string> { "496", "292" };
            var teamids = Teams.Keys;


            foreach (var team in teams) {
                ReadStatisticsFor (team, "0");
            }

            //foreach (var id in teamids) {
            //	ReadStatisticsFor (null, id);
            //}


            //var min = 1000000;
            //foreach (var game in games) {
            //	var newMin = game.LevelTimes.Min ((arg) => arg);
            //	if (newMin < min) {
            //		min = newMin;
            //		Console.WriteLine ($"The fastest level is in game {game.GameName} ({game.Id}) is {TimeSpan.FromSeconds (min)}");
            //	}
            //}
        }

        void ReadStatisticsFor (string team, string _id)
        {
            try {
                if (string.IsNullOrEmpty (team)) {
                    team = Teams [_id];
                }

                var result = string.Empty;

                List<GameByTeam> games;
                if (!string.IsNullOrEmpty (team)) {
                    games = GamesByTeam.FindAll (item => item.TeamName == team);
                } else {
                    games = GamesByTeam.FindAll (item => item.TeamId == _id);
                }

                result += $"Команда {team}";
                result += "\n";
                result += "\n";
                if (!games.Any ()) {
                    result = "Команда не сыграла ни одной игры.";
                    Console.WriteLine (result);
                    return;
                }

                var game = Games.First (item => item.Id.ToString () == games.First ().Id);
                result += $"Первая игра {game.Date} {game.Name}.";
                result += "\n";
                result += "\n";

                var seasonNumbers = games.Select (item => item.Season).Distinct ().ToList ();
                result += $"Участвовала в {seasonNumbers.Count} сезонах ({string.Join (", ", seasonNumbers)})";

                result += "\n";
                result += "\n";
                result += $"Участвовала в {games.Count} рейтинговых играх";

                var won = games.Where (item => item.Place == 1).ToList ();
                if (won.Count () > 0) {
                    result += "\n";
                    result += "\n";

                    result += $"Выиграно игр {won.Count} ";
                    foreach (var wongame in won) {
                        result += "\n";
                        var gameInfo = Games.First (item => item.Id.ToString () == wongame.Id);
                        result += $"        {gameInfo.Date} {gameInfo.Name} (сезон {wongame.Season})";
                    }
                } else {

                    result += "\n";
                    result += "\n";

                    var timeToLead = games.Select (item => item.TimeToLeader).Min ();
                    var gameToLead = games.Find (item => item.TimeToLeader == timeToLead);
                    var ttt = Games.First (item => item.Id.ToString () == gameToLead.Id);
                    result += $"Ближе всего к победе : {TimeSpan.FromSeconds (timeToLead)}, {ttt.Date} {gameToLead.GameName}";
                }

                result += "\n";
                result += "\n";
                result += "Какие места занимала команда за свою историю?";
                var placesDirty = games.Select (item => item.Place).ToList ();
                var placesClean = placesDirty.Distinct ().ToList ();
                placesClean.Sort ();

                for (var i = 1; i < 1000; i++) {
                    result += "\n";
                    result += $"        {i} место - {placesDirty.Count (item => item == i)} шт";
                    if (i >= placesClean.Last ()) {
                        break;
                    }
                }

                int maxAbsoluteGapInBestTime = 0;
                string maxAbsoluteGapInBestTimeGameId = string.Empty;
				double maxDependendGapInBestTime = 0;
                string maxDependendGapInBestTimeGameId = string.Empty;

                var ids = games.Select (item => item.Id).ToList ();
                var gameInfos = GamesByTeam.FindAll (item => ids.Contains (item.Id));
                var bestPlacesGameIds = new List<string> ();
                foreach (var g in gameInfos) {

                    for (var ind = 0; ind < g.LevelTimes.Count; ind++) {
                        if (g.BestTimes == null) {
                            break;
                        }

                        if (g.LevelTimes [ind] == g.BestTimes [ind]) {
                            if (g.TeamName == team)
                            {
                                bestPlacesGameIds.Add(g.Id);

                                var absoluteDiffBetweenFirstAndSecondTime = g.SecondBestTimes[ind] - g.BestTimes[ind];
                                if (absoluteDiffBetweenFirstAndSecondTime > maxAbsoluteGapInBestTime) {
                                    maxAbsoluteGapInBestTime = absoluteDiffBetweenFirstAndSecondTime;
                                    maxAbsoluteGapInBestTimeGameId = g.Id;
                                }

                                var dependantDiffBetweenFirstAndSecondTime = (g.SecondBestTimes[ind] - g.BestTimes[ind]) / ((double)g.BestTimes[ind]);
								if (dependantDiffBetweenFirstAndSecondTime > maxDependendGapInBestTime) {
                                    maxDependendGapInBestTime = dependantDiffBetweenFirstAndSecondTime;
                                    maxDependendGapInBestTimeGameId = g.Id;
                                }
                            }
                        }
                    }
                }


                if (bestPlacesGameIds.Count > 0) {
                    result += "\n";
                    result += "\n";
                    result += $"Лучшие времена: {bestPlacesGameIds.Count} шт";


                    var bestPlacesClean = bestPlacesGameIds.Distinct ().ToList ();
                    Dictionary<int, List<string>> bestpl = new Dictionary<int, List<string>> ();
                    List<int> bp = new List<int> ();
                    foreach (var id in bestPlacesClean) {
                        var count = bestPlacesGameIds.Count (item => item == id);
                        bp.Add (count);
                        if (bestpl.ContainsKey (count)) {
                            bestpl [count].Add (id);
                        } else {
                            bestpl.Add (count, new List<string> { id});
                        }
                    }

                    bp.Sort ();

                    var max = bp.Max ();



                    if (max == 8) {
                        Console.WriteLine (1);
                    }
                    for (var i = max; i > 0; i--) {
                        result += "\n";
                        var count = bp.Count (item => item == i);
                        result += $"        {i} лучших времен в { count} играх";
                        if (i <= bp.First ()) {
                            break;
                        }
                    }




                    result += "\n";
                    result += "\n";
                    result += $"Максимум лучших времен ({max}) в играх :";

                    var maxBPGameIds = bestpl.Where (item => item.Key == max).FirstOrDefault().Value;
                    foreach (var gMaxBP in maxBPGameIds) {
                        var g = Games.Find (item => gMaxBP == item.Id.ToString ());
                        result += "\n";
                        result += $"{g.Date} {g.Name}";
                    }

                }

                if (!string.IsNullOrEmpty(maxAbsoluteGapInBestTimeGameId))
                {
                    result += "\n";
                    result += "\n";
                    var minutes = maxAbsoluteGapInBestTime / 60;
                    var time = $"{minutes}:{maxAbsoluteGapInBestTime - minutes * 60}";

                    var g = Games.Find(item => maxAbsoluteGapInBestTimeGameId == item.Id.ToString());

                    result += $"Максимальный абсолютный отрыв в лучшем времени {time} до ближайшего соперника в игре {g.Date} {g.Name}";

					result += "\n";
					result += "\n";
					
                    g = Games.Find(item => maxDependendGapInBestTimeGameId == item.Id.ToString());

                    result += $"Максимальный относительный отрыв в лучшем времени:\n в {maxDependendGapInBestTime + 1} раз(а) быстрее ближайшего соперника в игре {g.Date} {g.Name}";

				}


                var minLevelTime = 1000000000;
                GameByTeam minLevelTimeGame = new GameByTeam ();

                var minTime = 10000000000;
                GameByTeam minTimeGame = new GameByTeam ();
                foreach (var ga in games) {
                    var newMin = ga.LevelTimes.Min ((arg) => arg);
                    if (newMin < minLevelTime) {
                        minLevelTime = newMin;
                        minLevelTimeGame = ga;
                    }

                    if (ga.CommonTime < minTime) {
                        minTime = ga.CommonTime;
                        minTimeGame = ga;
                    }
                }

                result += "\n";
                result += "\n";
                var lll = Games.First (item => item.Id.ToString () == minLevelTimeGame.Id);
                result += $"Самый быстрый быстрый уровень за всю историю: {TimeSpan.FromSeconds (minLevelTime)}, {lll.Date} {minLevelTimeGame.GameName}";

                result += "\n";
                result += "\n";
                lll = Games.First (item => item.Id.ToString () == minTimeGame.Id);
                result += $"Самая быстрая игра: {TimeSpan.FromSeconds (minTime)}, {lll.Date} {minTimeGame.GameName}";



                Console.WriteLine (result);
            } catch (Exception e) {
                Console.WriteLine ("!!!!!! ", team, _id);
            }
        }

        async Task ReadGameNames (List<Game> games)
        {
            foreach (var game in games) {
                var gameId = game.Id;
                var gameUrl = $"http://{Type}.dzzzr.ru/kaliningrad/?section=arc&what=stat&gmid={gameId}";
                var html = await HtmlReader.Instance.ReadHtml (gameUrl);
                var startString = "<h1>&laquo;";
                var endString = "&raquo;";
                var matches = GetMatches (html, startString, endString);
                foreach (var match in matches) {
                    game.Name = match;
                }

                startString = "<div class=Date>";
                endString = "</div>";
                matches = GetMatches (html, startString, endString);

                foreach (var match in matches) {
                    game.Date = match;
                    Console.WriteLine ($"------- {gameId} {game.Date} {game.Name}");
                }
            }
        }

        async Task<List<GameByTeam>> ReadTeamGames ()
        {
            var result = new List<GameByTeam> ();
            var count = 1;
            foreach (var game in Games) {
                Console.WriteLine ($"Read game {count++}/{Games.Count}");
                var url = $"http://{Type}.dzzzr.ru/kaliningrad/?section=arc&what=stat&gmid={game.Id}";
                var html = await HtmlReader.Instance.ReadHtml (url);

                var teamIds = GetInGameTeams (html);

                if (teamIds.Any ()) {
                    //Console.WriteLine ($"... New game type id = {game.Id}");

                    var places = GetPlaces (html);
                    var commonTimes = GetCommonTime (html);
                    var timeToPrev = GetTimeToPrevious (html);
                    var timeToLead = GetTimeToLeader (html);
                    var levels = GetLevels (html);

                    var ind = 0;
                    foreach (var teamId in teamIds) {
                        //не учитываем дисквалифицированных
                        if (ind >= places.Count) {
                            break;
                        }
                        var gameInfo = GetGameInfo (html, game, teamId);
                        gameInfo.TimeToPrevious = timeToPrev [ind];
                        gameInfo.TimeToLeader = timeToLead [ind];
                        gameInfo.CommonTime = commonTimes [ind];
                        gameInfo.Place = places [ind];
                        gameInfo.Season = SeasonGameIds [game.Id];
                        gameInfo.LevelTimes = levels [ind];
                        var teamName = Teams.ContainsKey (teamId) ? Teams [teamId] : DeletedTeamName;
                        Console.WriteLine ($"  {teamName} game id = {game.Id}, name = {game.Name}");
                        Console.WriteLine ($"  {teamName} season = {gameInfo.Season}");
                        Console.WriteLine ($"  {teamName} time to prev = {gameInfo.TimeToPrevious}");
                        Console.WriteLine ($"  {teamName} time to leader = {gameInfo.TimeToLeader}");
                        Console.WriteLine ($"  {teamName} common time = {gameInfo.CommonTime}");
                        Console.WriteLine ($"  {teamName} place = {places [ind]}");
                        Console.WriteLine ($"  {teamName} levels = {string.Join (", ", gameInfo.LevelTimes.ToArray ())}");
                        result.Add (gameInfo);

                        ind++;
                    }

                } else {
                    Console.WriteLine ($"... Old game type id = {game.Id}");
                }
            }
            return result;
        }

        List<List<int>> GetLevels (string html)
        {
            var result = new List<List<int>> ();
            var startString = "<!--cols ";
            var endString = "| colsend-->";
            var matches = GetMatches (html, startString, endString);
            var levels = matches.Select (item => item.Split ('|').ToList ()).ToList ();

            foreach (var level in levels) {
                level.RemoveAll (item => !item.All (char.IsDigit) || string.IsNullOrEmpty (item));
                var intLevels = level.Select (item => int.Parse (item)).ToList ();
                result.Add (intLevels);

            }
            return result;
        }

        GameByTeam GetGameInfo (string html, Game game, string teamId)

        {
            var info = new GameByTeam ();
            info.Id = game.Id.ToString ();
            info.TeamId = teamId;
            info.TeamName = (Teams.ContainsKey (teamId)) ? Teams [teamId] : DeletedTeamName;
            info.GameName = game.Name;
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
            if (result.Find (item => item.Contains ("/")) != null) {
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

        List<int> GetTimeToLeader (string html)
        {
            var startString = "<!--delta--><td>";
            var endString = "</td><!--deltaend-->";
            return GetMatches (html, startString, endString).Select (item => {
                return item.Contains ("-") ? "00:00:00" : item;
            }).Select (str => str.Substring (0, 8)).Select (item => GetTime (item)).ToList ();
        }


        int GetTime (string threePartTime)
        {
            TimeSpan ts = TimeSpan.Parse (threePartTime);
            return (int)ts.TotalSeconds;
        }


        Dictionary<int, int> SeasonGameIds = new Dictionary<int, int> ();

        async Task<List<Game>> ReadGames ()
        {
            var result = new List<int> ();
            var lastSeason = StartSeasonNumber;

            var lastYearAttempts = 0;
            for (var year = MIN_YEAR; year <= MAX_YEAR; year++) {
                Console.WriteLine ($".... Getting game ids for year {year} ....");
                for (var season = lastSeason; season < MAX_SEASON_NO; season++) {
                    //если нет таблички - значит в в этом году сезон другой.
                    var url = GetURL (year, season);
                    var html = await HtmlReader.Instance.ReadHtml (url);
                    if (IsEmptyTable (html)) {

                        if (year == MAX_YEAR && lastYearAttempts == 0) {
                            url = GetURL (year);
                            html = await HtmlReader.Instance.ReadHtml (url);
                            lastYearAttempts++;
                        } else {
                            break;
                        }
                    }

                    Console.WriteLine ($"  season {season} done");
                    var newGameIds = GetGameIdsFrom (html, season);
                    result.AddRange (newGameIds);
                    lastSeason++;
                }
            }
            result.Sort ();
            var games = result.Select (item => new Game { Id = item }).ToList ();
            await ReadGameNames (games);

            return games;
        }

        void ReadBestTimes ()
        {
            foreach (var game in Games) {
                var allCommandGames = GamesByTeam.Where (item => item.Id == game.Id.ToString ()).ToList ();
                var minimums = new List<int> ();
                var secondMinimums = new List<int> ();

                if (!allCommandGames.Any ()) continue;
                var firstGame = allCommandGames.First ();


                for (var i = 0; i < firstGame.LevelTimes.Count; i++) {
                    var level_i = allCommandGames.Select (item => item.LevelTimes [i]).ToList ();
                    var diffLevelTimeCount = level_i.Distinct().Count();
                    if (diffLevelTimeCount == 1 || diffLevelTimeCount == 2) {
                        minimums.Add (0);
                        secondMinimums.Add (0);
                    } else {
                        var min = level_i.Min();
                        minimums.Add (min);
                        var tmpForSecondMinimum = level_i;
                        tmpForSecondMinimum.Remove(min);
                        secondMinimums.Add(tmpForSecondMinimum.Min());
                    }
                }
                var selectedGames = GamesByTeam.FindAll (item => item.Id == game.Id.ToString ());
                foreach (var g in selectedGames) {
                    g.BestTimes = minimums;
                    g.SecondBestTimes = secondMinimums;
                }
            }
        }

        async Task<Dictionary<string, string>> ReadTeams ()
        {
            var teamsUrl = $"http://{Type}.dzzzr.ru/kaliningrad/?section=teams";
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
                    if (!SeasonGameIds.ContainsKey (id)) {
                        SeasonGameIds.Add (id, season);
                        result.Add (id);
                    }

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
