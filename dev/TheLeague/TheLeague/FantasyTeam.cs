using System.Collections.Generic;

namespace TheLeague
{
    public class FantasyTeam
    {
        public FantasyTeam()
        {
            Players = new List<FantasyPlayer>();
        }

        public int Id { get; set; }

        public string Manager { get; set; }

        public List<FantasyPlayer> Players { get; set; }
    }

    public class FantasyPlayer
    {
        public FantasyPlayer()
        {
        }

        public int Id { get; set; }

        public string Name { get; set; } 

        public FantasyStats SeasonStats { get; set; }

        //public FantasyStats Past30DayStats { get; set; }
    }

    public class FantasyStats
    {
        /*
          [0] = Games Played
          [3] = FG Attempted
          [4] = FG Made
          [5] = FG%
          [6] = FT Attempted
          [7] = FT Made
          [8] = FT%
          [10] = Threes Made
          [15] = Rebounds 
          [16] = Assists
          [17] = Steals
          [18] = Blocks
          [19] = Turnover
        */

        public FantasyStats(YahooSports.Api.Sports.Models.Player.PlayerStats.PlayerStat[] stats)
        {
            GamePlayed = ConvertStatValue(stats[0].Value);
            FGA = ConvertStatValue(stats[3].Value);
            FGM = ConvertStatValue(stats[4].Value);
            FTA = ConvertStatValue(stats[6].Value);
            FTM = ConvertStatValue(stats[7].Value);
            ThreesMade = ConvertStatValue(stats[10].Value);
            Rebounds = ConvertStatValue(stats[15].Value);
            Assists = ConvertStatValue(stats[16].Value);
            Steals = ConvertStatValue(stats[17].Value);
            Blocks = ConvertStatValue(stats[18].Value);
            Turnover = ConvertStatValue(stats[19].Value);
        }

        private int ConvertStatValue(string value)
        {
            if(value == "-")
            {
                return 0;
            }
            else
            {
                return int.Parse(value);
            }
        }

        public int GamePlayed { get; set; }

        public int FGA { get; set; }

        public int FGM { get; set; }

        public int FTA { get; set; }

        public int FTM { get; set; }

        public int ThreesMade { get; set; }

        public int Rebounds { get; set; }

        public int Assists { get; set; }

        public int Steals { get; set; }

        public int Blocks { get; set; }

        public int Turnover { get; set; }
    }
}
