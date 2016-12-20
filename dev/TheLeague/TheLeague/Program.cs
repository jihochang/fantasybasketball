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

        private static SportsProvider Client;


        public static void Main(string[] args)
        {
            SetupClient();

            FantasyContent leagueContent = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/league/364.l.45575");
            leagueContent = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/league/364.l.45575/settings");
            int numTeams = int.Parse(leagueContent.League.NumTeams);

            List<FantasyTeam> league = new List<FantasyTeam>();

            //FantasyTeam jiho = GetTeam("45575", "2");

            for(int i=1; i<=numTeams; i++)
            {
                league.Add(GetTeam("45575", i.ToString()));
            }

            // nba_id = 364
            //FantasyContent content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/game/nba");

            //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/league/364.l.45575");

            //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/team/364.l.45575.t.2/roster/players");

            //content = Client.ExecuteRequest<FantasyContent>("http://fantasysports.yahooapis.com/fantasy/v2/player/364.p.4612/stats");
        }

        private static FantasyTeam GetTeam(string leagueId, string teamId)
        {
            FantasyTeam team = new FantasyTeam();

            string teamUrl = "http://fantasysports.yahooapis.com/fantasy/v2/team/364.l." + leagueId + ".t." + teamId + "/roster/players";
            FantasyContent teamContent = Client.ExecuteRequest<FantasyContent>(teamUrl);

            team.Id = teamContent.Team.Id;
            team.Manager = teamContent.Team.Managers[0].Nickname;

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
