using System;
using System.Configuration;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using YahooSports.Api;
using YahooSports.Api.Exceptions;
using YahooSports.Api.Sports.Models;
using YahooSports.OAuthLib.Core;

namespace YahooSports.SportsUI {
    class Program {

        private static readonly string CONSUMER_KEY = ConfigurationManager.AppSettings["ConsumerKey"];
        private static readonly string CONSUMER_SECRET = ConfigurationManager.AppSettings["ConsumerSecret"];
        private const string TOKEN_FILE = "token";

        private static readonly IsolatedStorageFile Storage = IsolatedStorageFile.GetUserStoreForAssembly();

        private static AccessToken Token;
        private static SportsProvider Client;

        #region Example Synchronous Service Operations

        public static Player QuickPlayerTest()
        {
            try
            {
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/player/238.p.6619/stats");//http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.Player;
            }
            catch (SportsApiException ex)
            {
                Console.WriteLine("OAuth exception caught: " + ex.Message);
                Console.WriteLine("Refreshing token...");
                Client.RefreshToken();
                Console.WriteLine("Refresh token successful!");
                Console.WriteLine("Trying to execute normal authenticated request again!");
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/player/238.p.6619/stats");//"http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.Player;
            }
        }

        public static League QuickLeagueTest()
        {
            try
            {
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/league/238.l.178574/scoreboard;week=2");//http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.League;
            }
            catch (SportsApiException ex)
            {
                Console.WriteLine("OAuth exception caught: " + ex.Message);
                Console.WriteLine("Refreshing token...");
                Client.RefreshToken();//Token = oauthProvider.RefreshToken(Token);
                Console.WriteLine("Refresh token successful!");
                Console.WriteLine("Trying to execute normal authenticated request again!");
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/league/238.l.178574/scoreboard;week=2");//"http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.League;
            }
        }

        public static Team QuickTeamTest()
        {
            try
            {
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/team/223.l.431.t.9/roster/players");//http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.Team;
            }
            catch (SportsApiException ex)
            {
                Console.WriteLine("OAuth exception caught: " + ex.Message);
                Console.WriteLine("Refreshing token...");
                Client.RefreshToken();
                Console.WriteLine("Refresh token successful!");
                Console.WriteLine("Trying to execute normal authenticated request again!");
                FantasyContent content = Client.ExecuteRequest<FantasyContent>(@"http://fantasysports.yahooapis.com/fantasy/v2/team/223.l.431.t.9/roster/players");//"http://fantasysports.yahooapis.com/fantasy/v2/player/223.p.5479");//http://fantasysports.yahooapis.com/fantasy/v2/game/223/leagues;league_keys=223.l.431");//http://fantasysports.yahooapis.com/fantasy/v2/game/nba");
                return content.Team;
            }
        }

        #endregion

        [STAThread]
        static void Main(string[] args) {
            if (Storage.FileExists(TOKEN_FILE)) {
                try {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (Stream stream = Storage.OpenFile(TOKEN_FILE, FileMode.Open)) {
                        Token = formatter.Deserialize(stream) as AccessToken;
                    }
                } catch {
                    Console.WriteLine("ERROR: Error deserializing the token!");
                }
            }

            if (Token != null) {
                Client = new SportsProvider("", CONSUMER_KEY, CONSUMER_SECRET, Token);
                Player player = QuickPlayerTest();
                Console.WriteLine("Player name: " + player.Name);

                var league = QuickLeagueTest();
                Console.WriteLine("League name: " + league.Name);

                var team = QuickTeamTest();
                Console.WriteLine("Team name: " + team.Name);
            }
            else {

                string consumerKey = CONSUMER_KEY;
                string consumerSecret = CONSUMER_SECRET;
                string appUrl = string.Empty;
                string authCallback = "oob";
                //YahooProvider target = new YahooProvider(consumerKey, consumerSecret, appUrl, authCallback);
                
                //Func<string, string> userAuthCallback = url =>
                //{                    
                //        //const string loginUrl = "https://api.login.yahoo.com/oauth/v2/get_request_token";
                //        wb.Invoke(new Action(() => { wb.Navigate(url); })); //https://login.yahoo.com/config/login?
                //        if (wb.Url.ToString().StartsWith("https://login.yahoo.com/config/login")) { //https://api.login.yahoo.com/oauth/v2/request_auth?
                //            HtmlElement form = wb.Document.Forms["login_form"];
                //            //wb.Document.All.GetElementsByName(".save")[0].RaiseEvent("Click");
                //            HtmlElement userElement = form.All["username"];
                //            HtmlElement passElement = form.All["passwd"];
                //            form.InvokeMember("submit");
                //            if (wb.Url.ToString().StartsWith("https://api.login.yahoo.com/oauth/v2/request_auth?")) {
                //                form = wb.Document.Forms.GetElementsByName("rcForm")[0];
                //                form.InvokeMember("submit");
                //            } else
                //                throw new InvalidOperationException("uh oh... expected to get to the next url");
                //        }

                //        return wb.Url.Query;
                //}; // TODO: Initialize to an appropriate value
                //Token = target.Authenticate(userAuthCallback);

                Func<string, string> userAuthCallback = url =>
                {
                    using (Process p = new Process()) {
                        p.StartInfo = new ProcessStartInfo("iexplore.exe", url);
                        p.Start();
                        return Microsoft.VisualBasic.Interaction.InputBox("Enter key from (" + url + "):");
                    }
                };

                SportsProvider client = new SportsProvider(appUrl, CONSUMER_KEY, CONSUMER_SECRET);
                client.AuthenticateAsync(userAuthCallback, completeOperation => {
                    completeOperation(); // this must be called.
                    Token = client.Token;
                    Console.WriteLine("\nAsynchronous Authentication Operation completed successfully!\n");
                });
                Console.WriteLine("Waiting up to 60 seconds...");
                int count = 0;
                while (!client.IsAuthenticated && count < 60) {
                    Console.Write(". ");
                    Thread.Sleep(1000);
                    count++;
                }

                if (!client.IsAuthenticated)
                    return; // exit.

                using (Stream stream = Storage.OpenFile(TOKEN_FILE, FileMode.Create)) {
                    //DataContractSerializer serializer = new DataContractSerializer(typeof(AccessToken));    
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, Token);
                    //serializer.WriteObject(stream, Token);
                }
            }

            Console.WriteLine(String.Format("Token: {0}\nSecret: {1}\n", Token.Token, Token.TokenSecret));
            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
    }
}
