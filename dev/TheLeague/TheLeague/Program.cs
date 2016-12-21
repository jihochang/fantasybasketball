using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using YahooSports.Api;
using YahooSports.Api.Sports.Models;
using YahooSports.OAuthLib.Core;
using YahooSports.OAuthLib.Providers;

namespace TheLeague
{
    class Program
    {
        // Yahoo Developer Network
        // Registered Application "Basketball Research"
        // ConsumerKey and ConsumerSecret
        public const string ClientId = "dj0yJmk9VDFrMmpOck1zZ0ppJmQ9WVdrOVlWZG5NWGw1TldNbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD04Yw--";
        public const string ClientSecret = "b3d41f9349b9b818d7a1af5f100ee9e267768c13";

        private static readonly IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForAssembly();

        private const string TOKEN_FILE = "token";
        private static AccessToken Token;

        private static string CACHE_FILE = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\FantasyBasketball\leaguecache.txt";

        private static string DATA_FILE = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\FantasyBasketball\data.txt";

        private static SportsProvider Client;

        private const string LeagueId = "45575";

        private const bool UseCache = true;


        public static void Main(string[] args)
        {
            SetupClient();
            List<FantasyTeam> league = GetLeague(UseCache);

            List<List<string>> table = GenerateTeamTotals(league);

            WriteReport(DATA_FILE, table);
        }

        public static List<List<string>> GenerateTeamTotals(List<FantasyTeam> league)
        {
            List<List<string>> dataList = new List<List<string>>();
            List<List<double>> dataValues = new List<List<double>>();

            List<string> headers = new List<string> { "Team", "FG%", "FT%", "3s", "Points", "Assists", "Rebounds", "Steals", "Blocks", "Turnover" };
            dataList.Add(headers);

            foreach (FantasyTeam team in league)
            {
                double totalFGMade = 0;
                double totalFGAttempted = 0;

                double totalFTMade = 0;
                double totalFTAttempted = 0;

                double totalThrees = 0;
                double totalPoints = 0;
                double totalAssists = 0;
                double totalRebounds = 0;
                double totalSteals = 0;
                double totalBlocks = 0;
                double totalTurnovers = 0;

                int count = 0;
                foreach (FantasyPlayer player in team.Players)
                {
                    if (count > 9)
                    {
                        continue;
                    }
                    count++;

                    int games = player.SeasonStats.GamePlayed;

                    if (games == 0)
                    {
                        continue;
                    }

                    totalFGMade += player.SeasonStats.FGM;
                    totalFGAttempted += player.SeasonStats.FGA;

                    totalFTMade += player.SeasonStats.FTM;
                    totalFTAttempted += player.SeasonStats.FTA;

                    totalThrees += Math.Round((double)player.SeasonStats.ThreesMade / games, 2);
                    totalPoints += Math.Round((double)player.SeasonStats.PointsMade / games, 2);
                    totalAssists += Math.Round((double)player.SeasonStats.Assists / games, 2);
                    totalRebounds += Math.Round((double)player.SeasonStats.Rebounds / games, 2);
                    totalSteals += Math.Round((double)player.SeasonStats.Steals / games, 2);
                    totalBlocks += Math.Round((double)player.SeasonStats.Blocks / games, 2);
                    totalTurnovers += Math.Round((double)player.SeasonStats.Turnovers / games, 2);
                }

                double fgPercent = Math.Round(((totalFGMade / totalFGAttempted) * 100), 2);
                double ftPercent = Math.Round(((totalFTMade / totalFTAttempted) * 100), 2);

                List<double> dataValue = new List<double> { fgPercent, ftPercent, totalThrees, totalPoints, totalAssists, totalRebounds, totalSteals, totalBlocks, totalTurnovers};

                dataValues.Add(dataValue);

                List<string> dataString = new List<string> { team.Name, fgPercent.ToString() + "%", ftPercent.ToString() + "%", totalThrees.ToString(), totalPoints.ToString(),
                    totalAssists.ToString(), totalRebounds.ToString(), totalSteals.ToString(), totalBlocks.ToString(), totalTurnovers.ToString() };

                dataList.Add(dataString);
            }

            // EMPTY LINE
            dataList.Add(new List<string>());

            int cn = 0;
            foreach(List<double> row in dataValues)
            {
                string teamName = league[cn].Name;
                cn++;

                List<string> dataString = new List<string> { teamName };
                int count = 0;
                foreach (double stat in row)
                {
                    int rank = GetRank(stat, GetSingleStat(dataValues, count));
                    count++;
                    dataString.Add(rank.ToString());
                }

                dataList.Add(dataString);
            }

            return dataList;
        }
        
        private static List<double> GetSingleStat(List<List<double>> dataValues, int index)
        {
            List<double> stats = new List<double>();

            foreach (List<double> row in dataValues)
            {
                stats.Add(row[index]);
            }

            return stats;
        }

        private static int GetRank(double value, List<double> list)
        {
            list.Sort();

            int rank = list.Count;
            foreach(double element in list)
            {
                if(value == element)
                {
                    return rank;
                }

                rank--;
            }

            return 1;
        }

        private static void WriteReport(string fileName, List<List<string>> table)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (List<string> row in table)
                {
                    foreach(string value in row)
                    {
                        string temp = value.PadRight(25, ' ');
                        sw.Write(temp);
                    }

                    sw.WriteLine();
                }
            }
        }

        private static List<FantasyTeam> GetLeague(bool useCache)
        {
            if (useCache)
            {
                if (File.Exists(CACHE_FILE))
                {
                    using (StreamReader file = File.OpenText(CACHE_FILE))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        List<FantasyTeam> cacheLeague = (List<FantasyTeam>)serializer.Deserialize(file, typeof(List<FantasyTeam>));
                        return cacheLeague;
                    }
                }
            }

            FantasyContent leagueContent = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/league/364.l.45575");
            int numTeams = int.Parse(leagueContent.League.NumTeams);

            List<FantasyTeam> league = new List<FantasyTeam>();
            for (int i = 1; i < (numTeams+1); i++)
            {
                try
                {
                    FantasyTeam team = GetTeam("45575", i.ToString());
                    Thread.Sleep(3000);
                    league.Add(team);
                }
                catch (Exception)
                {
                }
            }

            using (StreamWriter file = File.CreateText(CACHE_FILE))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, league);
            }

            return league;
        }

        // nba_id = 364
        //FantasyContent content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/game/nba");

        //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/league/364.l.45575");

        //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/team/364.l.45575.t.2/roster/players");

        //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/player/364.p.4612/stats");

        private static FantasyTeam GetTeam(string leagueId, string teamId)
        {
            FantasyTeam team = new FantasyTeam();

            string teamUrl = "http://fantasysports.yahooapis.com/fantasy/v2/team/364.l." + leagueId + ".t." + teamId + "/roster/players";
            FantasyContent teamContent = Client.ExecuteRequest<FantasyContent>(teamUrl);

            team.Id = teamContent.Team.Id;
            team.Manager = teamContent.Team.Managers[0].Nickname;
            team.Name = teamContent.Team.Name;

            foreach (Team.Roster.TeamPlayer item in teamContent.Team.TeamRoster.Players)
            {
                FantasyPlayer player = new FantasyPlayer();
                player.Id = item.Id;
                player.Name = item.Name.Full;

                string playerUrl = "http://fantasysports.yahooapis.com/fantasy/v2/player/364.p." + player.Id + "/stats";
                FantasyContent playerContent = Client.ExecuteRequest<FantasyContent>(playerUrl);

                player.SeasonStats = new FantasyStats(playerContent.Player.PlayerStatistics.Stats);
                team.Players.Add(player);
            }

            return team;
        }

        private static void SetupClient()
        {
            GetToken(false);

            Client = new SportsProvider("", ClientId, ClientSecret, Token);
            Thread.Sleep(1000);

            try
            {
                FantasyContent content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
            }
            catch (Exception exception)
            {
                if(exception.HResult == -2146233088)
                {
                    GetToken(true);
                    Client = new SportsProvider("", ClientId, ClientSecret, Token);
                    Thread.Sleep(1000);
                    return;
                }

                throw;
            }
        }

        private static void GetToken(bool refreshToken)
        {
            if (Storage.FileExists(TOKEN_FILE))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (Stream stream = Storage.OpenFile(TOKEN_FILE, FileMode.Open))
                    {
                        Token = formatter.Deserialize(stream) as AccessToken;
                    }
                }
                catch
                {
                    Console.WriteLine("ERROR: Error deserializing the token!");
                }
            }

            if(Token == null || refreshToken)
            {
                string appUrl = string.Empty;
                string authCallback = "oob";

                YahooOAuthProvider provider = new YahooOAuthProvider(ClientId, ClientSecret, string.Empty, authCallback);

                Func<string, string> userAuthCallback = url =>
                {
                    using (Process p = new Process())
                    {
                        p.StartInfo = new ProcessStartInfo("iexplore.exe", url);
                        p.Start();
                        return Microsoft.VisualBasic.Interaction.InputBox("Enter key from (" + url + "):");
                    }
                };

                Token = provider.Authenticate(userAuthCallback);
            }

            using (Stream stream = Storage.OpenFile(TOKEN_FILE, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, Token);
            }

            Thread.Sleep(1000);
        }
    }
}
